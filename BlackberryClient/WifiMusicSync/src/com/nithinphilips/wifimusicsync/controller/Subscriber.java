package com.nithinphilips.wifimusicsync.controller;

import java.io.IOException;
import java.util.Enumeration;
import java.util.Vector;

import javax.microedition.io.Connector;
import javax.microedition.io.file.FileConnection;

import org.json.me.JSONException;
import org.json.me.JSONObject;

import com.nithinphilips.JsonHttpHelper;
import com.nithinphilips.wifimusicsync.model.PlaylistInfo;
import com.nithinphilips.wifimusicsync.model.PlaylistListResponse;
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
	
	public PlaylistInfo[] getAlbums() throws JSONException, IOException
    {
        return getPlaylists(server.getAlbumsUrl(), ".hpl");
    }
	
	public PlaylistInfo[] getArtists() throws JSONException, IOException
    {
        return getPlaylists(server.getArtistsUrl(), ".hpl");
    }
	
	public PlaylistInfo[] getPlaylists() throws JSONException, IOException
	{
	    return getPlaylists(server.getPlaylistsUrl(), ".m3u");
	}
	
	/**
	 * Get a list of all playlists on the server.
	 * @return
	 * @throws JSONException
	 * @throws IOException
	 */
	public PlaylistInfo[] getPlaylists(String url, String extension) throws JSONException, IOException{
	    // TODO: Choose URL based on the request type.
		byte[] httpBytes = JsonHttpHelper.httpGet(url);
		String s_response = null;
		if(httpBytes != null) s_response = new String(httpBytes);

		if ((s_response == null) || (s_response.compareTo("") == 0)) {
			log("Error: No response from server.");
			return null;
		} else {
			PlaylistListResponse response = PlaylistListResponse.fromJson(new JSONObject(s_response));
			PlaylistInfo[] playlists = response.getPlaylists();
			for (int i = 0; i < playlists.length; i++) {
			    playlists[i].setExtension(extension);
				playlists[i].setSelected(playlists[i].existsOnFileSystem(rootPath));
			}
			return playlists;
		}
	}

	public SyncResponse updateSubscription() throws JSONException, IOException {

		Vector playlists = new Vector();
		
		Vector files = findPlaylists(rootPath);
		if(files != null){
		    for (int i = 0; i < files.size(); i++)
            {
		        String file = (String)files.elementAt(i);
		        // NOTE: We assume that the extension is a dot + 3 chars 
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
			
			return null;
		} else {
			SyncResponse response = SyncResponse.fromJson(new JSONObject(
					s_response), server);
			
			return response;

//			if (response.getError() == SyncResponse.ERROR_NONE) {
//				// exec actions
//				PlaylistDownloader.executeActions(response.getActions());
//
//			} else {
//				log("Server Error " + response.getErrorMessage());
//			}
		}
	}

	private void log(String message) {
		
	}

	public static Vector findPlaylists(String root) throws IOException {
		FileConnection fileConnection = null;
		try {
			fileConnection = (FileConnection) Connector.open(root, Connector.READ);
			if (fileConnection.exists() && fileConnection.isDirectory()) {
                // NOTE: In other locations we assume that the extension is a dot + 3 chars
			    Vector result = new Vector();
				
				Enumeration m3uFiles = fileConnection.list("*.m3u", false);
				while (m3uFiles.hasMoreElements())
                    result.addElement(m3uFiles.nextElement());
				
				Enumeration hplFiles = fileConnection.list("*.hpl", false);
				while (hplFiles.hasMoreElements())
                    result.addElement(hplFiles.nextElement());
                
				return result;
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
