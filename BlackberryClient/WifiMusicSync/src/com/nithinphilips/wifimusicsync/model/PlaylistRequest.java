package com.nithinphilips.wifimusicsync.model;

import java.io.IOException;
import java.io.InputStream;
import java.util.Vector;

import javax.microedition.io.Connector;
import javax.microedition.io.file.FileConnection;

import org.json.me.JSONArray;
import org.json.me.JSONException;
import org.json.me.JSONObject;

import com.nithinphilips.ByteBuffer;
import com.nithinphilips.wifimusicsync.WifiMusicSync;

public class PlaylistRequest {
	
	String DeviceId = null;
	String PlaylistDevicePath = null;
	String DeviceMediaRoot = null;
    Vector PlaylistData = null;
    
    public String getPlaylistDevicePath() {
		return PlaylistDevicePath;
	}
    
	public void setPlaylistDevicePath(String playlistDevicePath) {
		PlaylistDevicePath = playlistDevicePath;
	}
	
	public String getDeviceId() {
		return DeviceId;
	}
	
	public void setDeviceId(String deviceId) {
		DeviceId = deviceId;
	}

	public String getDeviceMediaRoot() {
		return DeviceMediaRoot;
	}
	
	public void setDeviceMediaRoot(String deviceMediaRoot) {
		DeviceMediaRoot = deviceMediaRoot;
	}
	
	public Vector getPlaylistData() {
		return PlaylistData;
	}
	
	public void setPlaylistData(Vector playlistData) {
		PlaylistData = playlistData;
	}
	
	public void loadPlaylistData(String filePath) throws IOException{
		FileConnection connection = (FileConnection) Connector.open(filePath, Connector.READ);
		if (connection.exists()) {
			
			InputStream inStream = connection.openInputStream();
			ByteBuffer bb = new ByteBuffer(inStream);
			inStream.close();
			
			String playlistData = new String(bb.getBytes());
			Vector lineVec = WifiMusicSync.splitToVector(playlistData, "\n");
			
			setPlaylistData(lineVec);
		}
		connection.close();
	}

	public JSONObject toJsonObject() throws JSONException {
		JSONObject plsJsonReq = new JSONObject();
		plsJsonReq.put("DeviceId", DeviceId);
		plsJsonReq.put("PlaylistDevicePath", PlaylistDevicePath);
		plsJsonReq.put("DeviceMediaRoot", DeviceMediaRoot);

		if(PlaylistData != null){
			JSONArray plsJsonData = new JSONArray(PlaylistData);
			plsJsonReq.put("PlaylistData", plsJsonData);
		}else{
			plsJsonReq.put("PlaylistData", new JSONArray());
		}

		return plsJsonReq;
		
	}
	
}
