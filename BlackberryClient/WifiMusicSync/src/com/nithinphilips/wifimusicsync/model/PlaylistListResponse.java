package com.nithinphilips.wifimusicsync.model;

import org.json.me.JSONArray;
import org.json.me.JSONException;
import org.json.me.JSONObject;

public class PlaylistListResponse {
	
	public PlaylistInfo[] playlists;

	public PlaylistInfo[] getPlaylists() {
		return playlists;
	}

	public PlaylistListResponse(PlaylistInfo[] playlists) {
		super();
		this.playlists = playlists;
	}

	public static PlaylistListResponse fromJson(JSONObject json) throws JSONException{
		JSONArray tracks = json.getJSONArray("Tracks");
		PlaylistInfo[] playlists = new PlaylistInfo[tracks.length()];
		
		for (int i = 0; i < playlists.length; i++) {
			playlists[i] = PlaylistInfo.fromJson(tracks.getJSONObject(i));
		}
		
		return new PlaylistListResponse(playlists);
	}
	
}
