package com.nithinphilips.wifimusicsync.model;

import org.json.me.*;

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
		JSONArray tracks = json.getJSONArray("Playlists");
		PlaylistInfo[] playlists = new PlaylistInfo[tracks.length()];

		for (int i = 0; i < playlists.length; i++) {
			playlists[i] = PlaylistInfo.fromJson(tracks.getJSONObject(i));
		}

		return new PlaylistListResponse(playlists);
	}

	public String toString(){
		StringBuffer sb = new StringBuffer();
        for (int i = 0; i < playlists.length; i++) {
        	sb.append(playlists[i].toString());
        	sb.append("\n");
		}
        return sb.toString();
	}

}
