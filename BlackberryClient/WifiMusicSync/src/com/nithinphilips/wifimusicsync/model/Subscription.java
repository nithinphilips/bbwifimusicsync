package com.nithinphilips.wifimusicsync.model;

import java.io.IOException;
import java.util.Enumeration;
import java.util.Vector;

import javax.microedition.io.Connector;
import javax.microedition.io.file.FileConnection;

import org.json.me.JSONArray;
import org.json.me.JSONException;
import org.json.me.JSONObject;

public class Subscription extends Request {

	public Vector playlists;
	
	public Vector getPlaylists() {
		return playlists;
	}

	public void setPlaylists(Vector playlists) {
		this.playlists = playlists;
	}

	public JSONObject toJsonObject() throws JSONException {
		JSONObject plsJsonReq = new JSONObject();
		plsJsonReq.put("DeviceId", deviceId);
		plsJsonReq.put("DeviceMediaRoot", deviceMediaRoot);

		if(playlists != null){
			JSONArray plsJsonData = new JSONArray(playlists);
			plsJsonReq.put("Playlists", plsJsonData);
		}else{
			plsJsonReq.put("Playlists", new JSONArray());
		}

		return plsJsonReq;
	}
	
	

}
