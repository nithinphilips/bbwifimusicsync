package com.nithinphilips.wifimusicsync.model;

import org.json.me.JSONException;
import org.json.me.JSONObject;

public class PlaylistInfo {
	String name;
	int TrackCount;
	
	public String getName() {
		return name;
	}
	public int getTrackCount() {
		return TrackCount;
	}
	
	public PlaylistInfo(String name, int trackCount) {
		super();
		this.name = name;
		TrackCount = trackCount;
	}
	
	public static PlaylistInfo fromJson(JSONObject json) throws JSONException{
		return new PlaylistInfo(json.getString("Name"), json.getInt("TrackCount"));
	}
}
