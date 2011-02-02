package com.nithinphilips.wifimusicsync.model;

import java.io.IOException;
import java.io.InputStream;
import java.util.Vector;

import javax.microedition.io.Connector;
import javax.microedition.io.file.FileConnection;

import org.json.me.JSONArray;
import org.json.me.JSONException;
import org.json.me.JSONObject;

public class PlaylistRequest extends Request {
	
	
	String playlistDevicePath = null;
	
    Vector playlistData = null;
    
    public String getPlaylistDevicePath() {
		return playlistDevicePath;
	}
    
	public void setPlaylistDevicePath(String playlistDevicePath) {
		this.playlistDevicePath = playlistDevicePath;
	}
	
	public Vector getPlaylistData() {
		return playlistData;
	}
	
	public void setPlaylistData(Vector playlistData) {
		this.playlistData = playlistData;
	}
	

	public JSONObject toJsonObject() throws JSONException {
		JSONObject plsJsonReq = new JSONObject();
		plsJsonReq.put("DeviceId", deviceId);
		plsJsonReq.put("PlaylistDevicePath", playlistDevicePath);
		plsJsonReq.put("DeviceMediaRoot", deviceMediaRoot);

		if(playlistData != null){
			JSONArray plsJsonData = new JSONArray(playlistData);
			plsJsonReq.put("PlaylistData", plsJsonData);
		}else{
			plsJsonReq.put("PlaylistData", new JSONArray());
		}

		return plsJsonReq;
	}
	
}
