package com.nithinphilips.wifimusicsync;

import java.io.DataInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
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
import com.nithinphilips.wifimusicsync.model.PlaylistRequest;
import com.nithinphilips.wifimusicsync.model.SyncAction;
import com.nithinphilips.wifimusicsync.model.SyncResponse;

import net.rim.device.api.ui.MenuItem;
import net.rim.device.api.ui.component.Dialog;
import net.rim.device.api.ui.component.StandardTitleBar;
import net.rim.device.api.ui.component.progressindicator.ProgressIndicatorController;
import net.rim.device.api.ui.component.progressindicator.ProgressIndicatorModel;
import net.rim.device.api.ui.component.progressindicator.ProgressIndicatorView;
import net.rim.device.api.ui.container.MainScreen;


public class WifiMusicSyncScreen extends MainScreen {
	
	// ProgressView: http://docs.blackberry.com/en/developers/deliverables/17971/Indicate_progress_1210003_11.jsp
	// Please Wait Screen: http://supportforums.blackberry.com/t5/Java-Development/Sample-quot-Please-Wait-quot-screen-part-1/ta-p/493808
	
	
	static ProgressIndicatorView view;
	static ProgressIndicatorController controller;
	static ProgressIndicatorModel model;
	
	static String CLIENT_ID = "93a83cb36e87e65aL"; 
	
	public WifiMusicSyncScreen() {
		super(NO_VERTICAL_SCROLL);
		
		StandardTitleBar _titleBar = new StandardTitleBar();
		_titleBar.addClock();
		_titleBar.addSignalIndicator();
		_titleBar.addNotifications();
		this.setTitle(_titleBar);
		
//		view = new ProgressIndicatorView(0);
//		controller = new ProgressIndicatorController();
//		model = new ProgressIndicatorModel(50, 100, 0);
//
//
//		model.setController(controller);
//        view.setModel(model);
//        controller.setModel(model);
//        view.setController(controller);        
//        controller.setView(view);
//        view.setLabel("Percent completion");
//		
//		add(view);

		MenuItem menuItemSync = new MenuItem("Sync", 100000, 100) {
			public void run() {
				Thread t = new Thread() {
					public void run() {
						//String file = "file:///store/home/user/Stare at Ceiling.m3u";
						String file = "file:///SDCard/Blackberry/music/WiFiSync/Test.m3u";
						String root = "file:///SDCard/Blackberry/music/WiFiSync";
						
						//String file = "file:///SDCard/Test.m3u";
						//String root = "file:///SDCard/Blackberry/music/";

						//String file = "file:///store/home/user/WiFiSync/Test.m3u";
						//String root = "file:///store/home/user/";
						try {
							syncPlaylist("http://192.168.0.104:9000", root, file);
							
						} catch (Throwable t){
							
						}
//						catch (JSONException e) {
//							// TODO Auto-generated catch block
//							e.printStackTrace();
//						} catch (IOException e) {
//							System.out.println(e.getMessage());
//							// TODO Auto-generated catch block
//							e.printStackTrace();
//							Dialog.alert(e.toString() + "\n" +  e.getMessage());
//						}catch (IllegalArgumentException e) {
//							System.out.println(e.getMessage());
//							// TODO Auto-generated catch block
//							e.printStackTrace();
//							Dialog.alert(e.toString() + "\n" +  e.);
//						}
					}
				};
				t.run();
			}
		};
		
		addMenuItem(menuItemSync);
	}
	
	static void syncPlaylist(String serverUrl, String root, String playlist)
			throws JSONException, IOException {

		String mediaRoot;
		if (root.endsWith("/"))
			mediaRoot = root;
		else
			mediaRoot = root + "/";

		PlaylistRequest playlistRequest = new PlaylistRequest();
		playlistRequest.setDeviceId(CLIENT_ID);
		playlistRequest.setPlaylistDevicePath(playlist);
		playlistRequest.setDeviceMediaRoot(mediaRoot);
		playlistRequest.loadPlaylistData(playlist);

		String s_response = JsonHttpHelper.executeCommand(serverUrl + "/query", playlistRequest.toJsonObject().toString());
		
		SyncResponse response = SyncResponse.fromJson(new JSONObject(s_response), serverUrl);
		
		if(response.getError() == SyncResponse.ERROR_NONE){
			// exec actions
			executeActions(response.getActions());

			// update playlist
			downloadFile(response.getPlaylistServerUrl(), response.getPlaylistDevicePath(), true);
		}else{
			Dialog.alert("Error " + response.getErrorMessage());
		}
	}
	
	static void executeActions(SyncAction[] actions) throws JSONException, IOException {
			for (int i = 0; i < actions.length; i++) {
				if(actions[i].getType() == SyncAction.ADD){
					String trackUrl = actions[i].getTrackUrl(); 
					createDirectoryTree(trackUrl);
					downloadFile(trackUrl, actions[i].getDeviceLocation());
				}else if(actions[i].getType() == SyncAction.REMOVE){
					deleteFile(actions[i].getDeviceLocation());
					// TODO: Delete empty directory trees too
				}
			}
	}
	
	static void deleteFile(String fileName) throws IOException
	{
		FileConnection fileConnection = null;
		try{
			fileConnection = (FileConnection) Connector.open(fileName);
			if(fileConnection.exists()) fileConnection.delete();
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
	
	public static final int BUFFER_SIZE = 512;
	
	static void createDirectoryTree(String fileName) throws IOException{
		// Create directories
		String[] segments = WifiMusicSync.getDirectories("file:///", fileName);
		for (int i = 0; i < segments.length - 1; i++) {
			//Dialog.alert(segments[i]);
			FileConnection dir = (FileConnection) Connector.open(segments[i], Connector.READ_WRITE);
			if (!dir.exists()) dir.mkdir();
		}
	}
	
	static void downloadFile(String url, String fileName) throws IOException
	{
		downloadFile(url, fileName, false);
	}
	
	static void downloadFile(String url, String fileName, boolean forceOverwrite) throws IOException
	{
		Dialog.alert("Downloading: " + fileName + " from: " + url);
		
		HttpConnection httpConnection = null;
		FileConnection fileConnection = null;
		
		InputStream httpInStream = null;
		OutputStream fileOutStream = null;
		
		try {
			httpConnection = (HttpConnection) Connector.open(url + JsonHttpHelper.URL_SUFFIX);
			// HTTP Request
			httpConnection.setRequestMethod(HttpConnection.GET);
			httpConnection.setRequestProperty("Connection", "close");

			int status = httpConnection.getResponseCode();
			
			if (status == HttpConnection.HTTP_OK) {
				fileConnection = (FileConnection) Connector.open(fileName, Connector.READ_WRITE);
				
				if (fileConnection.exists()){
					if(forceOverwrite){
						fileConnection.delete();
					}else{
						try{
							long contentLength = Long.parseLong(httpConnection.getHeaderField("Content-Length"));
							Dialog.alert("Sever: " + Long.toString(contentLength) + " Client: " + Long.toString(fileConnection.fileSize()));
							if(contentLength != fileConnection.fileSize()){
								fileConnection.delete();		
							}else{
								// Same file. No skip download
								httpConnection.close();
								Dialog.alert("Skip download.");
								return;
							}
						}catch(Exception ex){
							// Delete the file, to be safe
							Dialog.alert("Exception while .");
							if (fileConnection.exists()) fileConnection.delete();
						}
					}	
				}
				
				fileConnection.create();
				
				//Dialog.alert("Creating " + fileName);
				
				httpInStream = httpConnection.openInputStream();
				fileOutStream = fileConnection.openOutputStream();

				byte[] readBuf = new byte[BUFFER_SIZE];
				while (true) {
					int read = httpInStream.read(readBuf);
					//Dialog.alert("Read " + Integer.toString(read));
					if (read == -1) break;
					fileOutStream.write(readBuf, 0, read);
				}
				
				fileOutStream.flush();

				Dialog.alert("Done: " + fileName);
			}
		} finally {
			try {
				if (httpInStream != null) httpInStream.close();
			} catch (Exception error) {}
			
			try {
				if (fileOutStream!= null) fileOutStream.close();
			} catch (Exception error) {}
			
			try {
				if (httpConnection != null) httpConnection.close();
			} catch (Exception error) {}
			
			try {
				if (fileConnection != null) fileConnection.close();
			} catch (Exception error) {}
		}
	}
}
