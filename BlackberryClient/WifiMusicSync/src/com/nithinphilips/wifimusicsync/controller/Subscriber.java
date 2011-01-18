package com.nithinphilips.wifimusicsync.controller;

import java.io.IOException;
import java.util.Enumeration;
import java.util.Vector;

import javax.microedition.io.Connector;
import javax.microedition.io.file.FileConnection;

import net.rim.device.api.ui.component.Dialog;

import org.json.me.JSONException;
import org.json.me.JSONObject;

import com.nithinphilips.JsonHttpHelper;
import com.nithinphilips.wifimusicsync.model.PlaylistInfo;
import com.nithinphilips.wifimusicsync.model.PlaylistListResponse;
import com.nithinphilips.wifimusicsync.model.PlaylistRequest;
import com.nithinphilips.wifimusicsync.model.Subscription;
import com.nithinphilips.wifimusicsync.model.SyncResponse;
import com.nithinphilips.wifimusicsync.model.UrlBuilder;

public class Subscriber {

	UrlBuilder server;
	String rootPath;
	String clientId;

	public Subscriber(String serverUrl, String rootPath, String clientId) {
		this.server = new UrlBuilder(serverUrl);
		this.rootPath = rootPath;
		this.clientId = clientId;
	}
	
	
	/**
	 * Get a list of all playlists on the server.
	 * @return
	 * @throws JSONException
	 * @throws IOException
	 */
	public PlaylistInfo[] getPlaylists() throws JSONException, IOException{
		byte[] httpBytes = JsonHttpHelper.httpGet((server.getPlaylistsUrl()));
		String s_response = null;
		if(httpBytes != null) s_response = new String(httpBytes);

		if ((s_response == null) || (s_response.compareTo("") == 0)) {
			log("Error: No response from server.");
			return null;
		} else {
			PlaylistListResponse response = PlaylistListResponse.fromJson(new JSONObject(s_response));
			PlaylistInfo[] playlists = response.getPlaylists();
			for (int i = 0; i < playlists.length; i++) {
				playlists[i].setSelected(playlists[i].existsOnFileSystem(rootPath));
			}
			return playlists;
		}
	}

	public void updateSubscription() throws JSONException, IOException {

		Vector playlists = new Vector();
		Enumeration files = findPlaylists(rootPath);
		
		if(files != null){
			while (files.hasMoreElements()) {
				String file = (String)files.nextElement();
				playlists.addElement(file.substring(0, file.length() - 4));
			}
		}

		Subscription subscription = new Subscription();
		subscription.setDeviceId(clientId);
		subscription.setDeviceMediaRoot(rootPath);
		subscription.setPlaylists(playlists);

		String s_response = JsonHttpHelper.executeCommand(server.getSubscribe(), subscription.toJsonObject().toString());

		if ((s_response == null) || (s_response.compareTo("") == 0)) {
			log("Error: No response from server.");
		} else {
			SyncResponse response = SyncResponse.fromJson(new JSONObject(
					s_response), server);

			if (response.getError() == SyncResponse.ERROR_NONE) {
				// exec actions
				PlaylistDownloader.executeActions(response.getActions());

			} else {
				log("Server Error " + response.getErrorMessage());
			}
		}
	}

	private void log(String message) {
		
	}

	public static Enumeration findPlaylists(String root) throws IOException {
		FileConnection fileConnection = null;
		try {
			fileConnection = (FileConnection) Connector.open(root, Connector.READ);
			if (fileConnection.exists() && fileConnection.isDirectory()) {
				return fileConnection.list("*.m3u", false);
			} else {
				return null;
			}
		} finally {
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
