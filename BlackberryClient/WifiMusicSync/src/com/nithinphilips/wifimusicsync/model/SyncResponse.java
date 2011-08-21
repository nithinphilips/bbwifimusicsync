package com.nithinphilips.wifimusicsync.model;

import org.json.me.*;

public class SyncResponse {

	public final static int ERROR_NONE = 0;
	public final static int ERROR_PLAYLISTNOTFOUND = 100;

	int error = 0;
	String errorMessage = null;
	String serverId = null;
	String playlistServerPath = null;
    String playlistDevicePath = null;
    SyncAction[] actions = null;

    public int getError() {
		return error;
	}

	public String getErrorMessage() {
		return errorMessage;
	}

	public String getServerId() {
		return serverId;
	}

	public String getPlaylistServerUrl() {
		return playlistServerPath;
	}

	public String getPlaylistDevicePath() {
		return playlistDevicePath;
	}

	public SyncAction[] getActions() {
		return actions;
	}

	public static SyncResponse fromJson(JSONObject json, UrlBuilder server) throws JSONException{
    	SyncResponse response = new SyncResponse();

    	response.serverId = json.getString("ServerId");
    	response.error = json.getInt("Error");
    	if(response.error > 0){
    		// An error happened. No data.
    		response.errorMessage = json.getString("ErrorMessage");
    	}else{
    		response.playlistServerPath = server.getFullUrl(json.getString("PlaylistServerPath"));
    		response.playlistDevicePath = json.getString("PlaylistDevicePath");

    		JSONArray actionsJson = json.getJSONArray("Actions");
    		SyncAction[] actions = new SyncAction[actionsJson.length()];
    		for (int i = 0; i < actions.length; i++) {
    			actions[i] = SyncAction.fromJson(actionsJson.getJSONObject(i), server);
			}
    		response.actions = actions;
    	}

    	return response;
    }

	public String toString()
	{
		StringBuffer sb = new StringBuffer();
		if (error != 0)
        {
			sb.append("Error: ");
	        sb.append(error);
	        sb.append("\n");

	        sb.append("ErrorMessage: ");
	        sb.append(errorMessage);
	        sb.append("\n");
        }

        sb.append("ServerId: ");
        sb.append(serverId);
        sb.append("\n");

        sb.append("PlaylistServerPath: ");
        sb.append(playlistServerPath);
        sb.append("\n");

        sb.append("PlaylistDevicePath: ");
        sb.append(playlistDevicePath);
        sb.append("\n");

        sb.append("Actions: ");
        sb.append("\n");
        for (int i = 0; i < actions.length; i++) {
        	sb.append("    " + actions[i].toString());
        	sb.append("\n");
		}
        return sb.toString();
	}

}
