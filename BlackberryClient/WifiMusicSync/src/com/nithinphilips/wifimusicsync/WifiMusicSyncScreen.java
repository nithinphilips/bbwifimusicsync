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

import com.nithinphilips.ByteBuffer;
import com.nithinphilips.Debug;
import com.nithinphilips.JsonHttpHelper;
import com.nithinphilips.wifimusicsync.components.InputDialog;
import com.nithinphilips.wifimusicsync.components.PlaylistSelectionDialog;
import com.nithinphilips.wifimusicsync.controller.PlaylistDownloader;
import com.nithinphilips.wifimusicsync.controller.Subscriber;
import com.nithinphilips.wifimusicsync.model.PlaylistInfo;
import com.nithinphilips.wifimusicsync.model.PlaylistListResponse;
import com.nithinphilips.wifimusicsync.model.PlaylistRequest;
import com.nithinphilips.wifimusicsync.model.Subscription;
import com.nithinphilips.wifimusicsync.model.SyncAction;
import com.nithinphilips.wifimusicsync.model.SyncResponse;
import com.nithinphilips.wifimusicsync.model.UrlBuilder;

import net.rim.device.api.ui.MenuItem;
import net.rim.device.api.ui.UiApplication;
import net.rim.device.api.ui.component.Dialog;
import net.rim.device.api.ui.component.LabelField;
import net.rim.device.api.ui.component.RichTextField;
import net.rim.device.api.ui.component.StandardTitleBar;
import net.rim.device.api.ui.container.MainScreen;


public class WifiMusicSyncScreen extends MainScreen {
	
	// ProgressView: http://docs.blackberry.com/en/developers/deliverables/17971/Indicate_progress_1210003_11.jsp
	// Please Wait Screen: http://supportforums.blackberry.com/t5/Java-Development/Sample-quot-Please-Wait-quot-screen-part-1/ta-p/493808
	
	static String CLIENT_ID = "93a83cb36e87e65aL";
	String root = "file:///SDCard/Blackberry/music/WiFiSync/";
	String serverUrl = "http://192.168.0.104:9000";
	
	RichTextField statusField;
	
	public WifiMusicSyncScreen() {
		super(VERTICAL_SCROLL);
		
		StandardTitleBar _titleBar = new StandardTitleBar();
		_titleBar.addClock();
		_titleBar.addSignalIndicator();
		_titleBar.addNotifications();
		this.setTitle(_titleBar);
		
		statusField = new RichTextField("Ready.");
		
		add(statusField);
		
		MenuItem menuItemMakePlaylist = new MenuItem("Add Playlist", 100000, 100) {
			public void run(){
				String choices[] = {"Ok","Cancel"}; 
				int values[] = {Dialog.OK,Dialog.CANCEL}; 
				final InputDialog diag = new InputDialog(); 
				UiApplication.getUiApplication().invokeLater(
						new Runnable() { 
							public void run() { 
								PlaylistInfo[] playlists;
								try {
									
									Subscriber subscriber = new Subscriber(serverUrl, root, CLIENT_ID);
									
									playlists = subscriber.getPlaylists();
								
									if(playlists != null){ 
										PlaylistSelectionDialog diag = new PlaylistSelectionDialog(playlists);
										UiApplication.getUiApplication().pushModalScreen(diag); 
										if(diag.getResult() == Dialog.OK) {
											// 1. Add newly checked playlists
											for (int i = 0; i < playlists.length; i++) {
												if(playlists[i].isSelected())
													playlists[i].createOnFileSystem(root);
												else
													playlists[i].deleteOnFileSystem(root);
											}
											
											// 2. A. Delete unchecked playlists B. Cleanup unreferenced tracks
											subscriber.updateSubscription();
										}
									}
								} catch (JSONException e) {
									log(e.toString());
								} catch (IOException e) {
									log(e.toString());
								}
				} }); 
			}
		};
		
		
		MenuItem menuItemSync = new MenuItem("Sync", 100000, 100) {
			public void run() {
				Thread t = new Thread() {
					public void run() {
						
						try {
							Enumeration playlists = Subscriber.findPlaylists(root);
							
							if(playlists != null){
								while (playlists.hasMoreElements()) {
									String playlist = root + (String) playlists.nextElement();
									//log(playlist);
									PlaylistDownloader downloader =  new PlaylistDownloader(serverUrl, 
												playlist, 
												root, 
												CLIENT_ID);
									downloader.syncPlaylist();
								}
							}else{
								log("No playlists found");
							}
//						} catch (Throwable t){
//							
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
			}
		};
		
		addMenuItem(menuItemMakePlaylist);
		addMenuItem(menuItemSync);
	}
	
	
	
	private void log(final String message)
	{
		UiApplication.getUiApplication().invokeLater(new Runnable() {
			public void run() {
				Dialog.alert(message);
				statusField.setText(statusField.getText() + "\n" + message);
			}
		});
	}
	
}
