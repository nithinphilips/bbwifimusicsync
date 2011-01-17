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
import com.nithinphilips.wifimusicsync.controller.PlaylistDownloader;
import com.nithinphilips.wifimusicsync.model.PlaylistRequest;
import com.nithinphilips.wifimusicsync.model.SyncAction;
import com.nithinphilips.wifimusicsync.model.SyncResponse;

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
		

		MenuItem menuItemSync = new MenuItem("Sync", 100000, 100) {
			public void run() {
				Thread t = new Thread() {
					public void run() {
						String root = "file:///SDCard/Blackberry/music/WiFiSync/";
						
						try {
							Enumeration playlists = findPlaylists(root);
							
							if(playlists != null){
								while (playlists.hasMoreElements()) {
									String playlist = root + (String) playlists.nextElement();
									log("------------------------");
									log(playlist);
									PlaylistDownloader downloader = 
										new PlaylistDownloader(
												"http://192.168.0.104:9000", 
												playlist, 
												root, 
												CLIENT_ID);
									downloader.syncPlaylist();
								}
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
		
		addMenuItem(menuItemSync);
	}
	
	private void log(final String message)
	{
		//Dialog.alert(message);
		UiApplication.getUiApplication().invokeLater(new Runnable() {
			public void run() {
				statusField.setText(statusField.getText() + "\n" + message);
			}
		});
	}
	
	Enumeration findPlaylists(String root) throws IOException
	{
		FileConnection fileConnection = null;
		try{
			fileConnection = (FileConnection) Connector.open(root, Connector.READ);
			if(fileConnection.exists() && fileConnection.isDirectory())
			{
				return fileConnection.list("*.m3u", false);
			}else{
				return null;
			}
		}finally {
			if (fileConnection != null) {
				try {
					fileConnection.close();
				} catch (Exception error) {
					/* log error */
				}
			}
		}
	}
	
	
}
