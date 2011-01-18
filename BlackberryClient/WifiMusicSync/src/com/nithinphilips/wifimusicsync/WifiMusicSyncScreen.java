package com.nithinphilips.wifimusicsync;

import java.io.DataInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.Enumeration;
import java.util.Vector;

import javax.microedition.io.Connector;
import javax.microedition.io.HttpConnection;
import javax.microedition.io.file.FileConnection;

import org.json.me.JSONArray;
import org.json.me.JSONException;
import org.json.me.JSONObject;

import com.fairview5.keepassbb2.common.ui.ProgressDialog;
import com.nithinphilips.ByteBuffer;
import com.nithinphilips.Debug;
import com.nithinphilips.JsonHttpHelper;
import com.nithinphilips.wifimusicsync.components.InputDialog;
import com.nithinphilips.wifimusicsync.components.PlaylistSelectionDialog;
import com.nithinphilips.wifimusicsync.components.ProgressListModel;
import com.nithinphilips.wifimusicsync.components.WifiMusicSyncProperties;
import com.nithinphilips.wifimusicsync.controller.PlaylistDownloader;
import com.nithinphilips.wifimusicsync.controller.Subscriber;
import com.nithinphilips.wifimusicsync.model.PlaylistInfo;
import com.nithinphilips.wifimusicsync.model.PlaylistListResponse;
import com.nithinphilips.wifimusicsync.model.PlaylistRequest;
import com.nithinphilips.wifimusicsync.model.Subscription;
import com.nithinphilips.wifimusicsync.model.SyncAction;
import com.nithinphilips.wifimusicsync.model.SyncResponse;
import com.nithinphilips.wifimusicsync.model.UrlBuilder;

import net.rim.device.api.ui.DrawStyle;
import net.rim.device.api.ui.MenuItem;
import net.rim.device.api.ui.TransitionContext;
import net.rim.device.api.ui.Ui;
import net.rim.device.api.ui.UiApplication;
import net.rim.device.api.ui.UiEngineInstance;
import net.rim.device.api.ui.component.Dialog;
import net.rim.device.api.ui.component.LabelField;
import net.rim.device.api.ui.component.ListField;
import net.rim.device.api.ui.component.RichTextField;
import net.rim.device.api.ui.component.StandardTitleBar;
import net.rim.device.api.ui.component.Status;
import net.rim.device.api.ui.container.MainScreen;


public class WifiMusicSyncScreen extends MainScreen {
	
	// ProgressView: http://docs.blackberry.com/en/developers/deliverables/17971/Indicate_progress_1210003_11.jsp
	// Please Wait Screen: http://supportforums.blackberry.com/t5/Java-Development/Sample-quot-Please-Wait-quot-screen-part-1/ta-p/493808
	
	ListField myListView;
	ProgressListModel myListModel;
	
	public WifiMusicSyncScreen() {
		super(VERTICAL_SCROLL);
		
		StandardTitleBar _titleBar = new StandardTitleBar();
		_titleBar.addTitle("Music Sync");
		_titleBar.addNotifications();
		_titleBar.addSignalIndicator();
		this.setTitle(_titleBar);

		myListView = new ListField();
		myListView.setEmptyString("Choose 'Sync' to start syncing.", DrawStyle.HCENTER);
		myListModel = new ProgressListModel(myListView, new Vector());
		
		add(myListView);
		
		MenuItem menuItemOptions = new MenuItem("Options", 100000, 100) {
			public void run() {
				pushSettingsScreen();
			}
		};
		
		MenuItem menuItemMakePlaylist = new MenuItem("Add Playlist", 100000, 100) {
			public void run(){
				subscribe();
			}
		};
		
		
		MenuItem menuItemSync = new MenuItem("Sync", 100000, 100) {
			public void run() {
				sync();
			}
		};
		
		MenuItem menuItemTest = new MenuItem("Test", 100000, 100) {
			public void run() {
				Thread t = new Thread() {
					public void run() {
						final SyncAction addaction = new SyncAction();
						final SyncAction remaction = new SyncAction();
						
						addaction.setDeviceLocation("file:///SDCard/test.mp3");
						addaction.setType(SyncAction.ADD);
						
						remaction.setDeviceLocation("file:///SDCard/test2/test.mp3");
						remaction.setType(SyncAction.REMOVE);
						
						addaction.setStatus("Queued");
						remaction.setStatus("Queued");
						
						
						UiApplication.getUiApplication().invokeLater(new Runnable() {
							public void run() {
								myListModel.insert(addaction, myListModel.size());
								myListModel.insert(remaction, myListModel.size());
							}
						});
						
					}
				};
				t.start();
			}
		};
		
		addMenuItem(menuItemOptions);
		addMenuItem(menuItemMakePlaylist);
		addMenuItem(menuItemSync);
		addMenuItem(menuItemTest);
	}
	
	void subscribe() {

		ProgressDialog.prepareModal("Getting playlists...");

		Thread t = new Thread() {
			public void run() {
				try {
					final PlaylistInfo[] playlists;
					final WifiMusicSyncProperties props = WifiMusicSyncProperties.fetch();

					final Subscriber subscriber = new Subscriber(
							props.getServerUrl(), props.getLocalStoreRoot(),
							props.getClientId());

					playlists = subscriber.getPlaylists();

					ProgressDialog.closeProgress();

					if (playlists != null) {
						UiApplication.getUiApplication().invokeLater(
								new Runnable() {
									public void run() {
										try {
											PlaylistSelectionDialog diag = new PlaylistSelectionDialog(playlists);
											UiApplication.getUiApplication().pushModalScreen(diag);
											if (diag.getResult() == Dialog.OK) {
												// 1. Add newly checked
												// playlists
												for (int i = 0; i < playlists.length; i++) {
													if (playlists[i]
															.isSelected())
														playlists[i]
																.createOnFileSystem(props
																		.getLocalStoreRoot());
													else
														playlists[i]
																.deleteOnFileSystem(props
																		.getLocalStoreRoot());
												}

												// 2. A. Delete unchecked
												// playlists B. Cleanup
												// unreferenced tracks
												subscriber.updateSubscription();
											}
										} catch (JSONException e) {
											log(e.toString());
										} catch (IOException e) {
											log(e.toString());
										}
									}
								});
						
					}else{
						
					}

				} catch (JSONException e) {
					log(e.toString());
				} catch (IOException e) {
					log(e.toString());
				}

			}
		};
		t.start();
		ProgressDialog.doModal();

	}
	
	void sync(){
		
		ProgressDialog.prepareModal("Connecting to server...");
		Thread t = new Thread() {
			public void run() {
				try {
					WifiMusicSyncProperties props = WifiMusicSyncProperties.fetch();
					Enumeration playlists = Subscriber.findPlaylists(props.getLocalStoreRoot());
					if(playlists != null){
						while (playlists.hasMoreElements()) {
							String playlist = props.getLocalStoreRoot() + (String) playlists.nextElement();
							
							PlaylistDownloader downloader =  new PlaylistDownloader(props.getServerUrl(), 
										playlist, 
										props.getLocalStoreRoot(), 
										props.getClientId());
							
							SyncResponse response =  downloader.getResponse();
							
							ProgressDialog.closeProgress();
							
							if(response != null){
								final SyncAction[] actions = response.getActions();
								
								UiApplication.getUiApplication().invokeLater(new Runnable() {
									public void run() {
										myListModel.erase();
										
										for (int i = 0; i < actions.length; i++) {
											myListModel.insert(actions[i]);
										}
									}
								});
								
								downloader.handleResponse();
							}
							
						}
					}else{
						log("No playlists found");
					}
				}
				catch (JSONException e) {
					// TODO Auto-generated catch block
					log(e.toString());
				} catch (IOException e) {
					log(e.toString());
				}catch (IllegalArgumentException e) {
					log(e.toString());
				}
			}
		};
		t.start();
		ProgressDialog.doModal();
	}
	
	void pushSettingsScreen(){
		
		SettingsScreen screen = new SettingsScreen();
		
		TransitionContext transitionContextIn;
        TransitionContext transitionContextOut;
        UiEngineInstance engine = Ui.getUiEngineInstance();
        
		transitionContextIn = new TransitionContext(TransitionContext.TRANSITION_SLIDE);
        transitionContextIn.setIntAttribute(TransitionContext.ATTR_DURATION, 250);
        transitionContextIn.setIntAttribute(TransitionContext.ATTR_DIRECTION, TransitionContext.DIRECTION_LEFT);              
        
        transitionContextOut = new TransitionContext(TransitionContext.TRANSITION_SLIDE);
        transitionContextOut.setIntAttribute(TransitionContext.ATTR_DURATION, 250);
        transitionContextOut.setIntAttribute(TransitionContext.ATTR_DIRECTION, TransitionContext.DIRECTION_RIGHT);                                                            
        transitionContextOut.setIntAttribute(TransitionContext.ATTR_KIND, TransitionContext.KIND_OUT);                
                                                                    
        engine.setTransition(null, screen, UiEngineInstance.TRIGGER_PUSH, transitionContextIn);
        engine.setTransition(screen, null, UiEngineInstance.TRIGGER_POP, transitionContextOut);
        
        UiApplication.getUiApplication().pushModalScreen(screen);  
	}
	
	
	private void log(final String message)
	{
//		UiApplication.getUiApplication().invokeLater(new Runnable() {
//			public void run() {
//				//Dialog.alert(message);
//				//statusField.setText(statusField.getText() + "\n" + message);
//			}
//		});
	}
	
}
