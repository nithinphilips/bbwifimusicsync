package com.nithinphilips.wifimusicsync.model;

import java.io.IOException;

import javax.microedition.io.Connector;
import javax.microedition.io.file.FileConnection;

import org.json.me.JSONException;
import org.json.me.JSONObject;

import com.nithinphilips.wifimusicsync.controller.PlaylistDownloader;

public class PlaylistInfo {
	
	String name;
	String displayName;
	int trackCount;
	boolean selected;
	
	public boolean isSelected() {
		return selected;
	}
	
	public void setSelected(boolean selected) {
		this.selected = selected;
	}
	
	public String getDisplayName() {
		return displayName;
	}
	
	public String getName() {
		return name;
	}
	
	public int getTrackCount() {
		return trackCount;
	}
	
	public boolean existsOnFileSystem(String root) throws IOException{
		FileConnection file = (FileConnection) Connector.open(getPath(root), Connector.READ);
		try{
			return file.exists();
		}finally{
			file.close();
		}
	}
	
	public void createOnFileSystem(String root) throws IOException{
		String path = getPath(root);
		PlaylistDownloader.createDirectoryTree(path);
		
		FileConnection file = (FileConnection) Connector.open(path, Connector.READ_WRITE);
		if (!file.exists())
			file.create();
		
		file.close();
	}
	
	public void deleteOnFileSystem(String root) throws IOException{
		String path = getPath(root);
		PlaylistDownloader.createDirectoryTree(path);
		
		FileConnection file = (FileConnection) Connector.open(path, Connector.READ_WRITE);
		if (file.exists())
			file.delete();
		
		file.close();
	}
	
	String getPath(String root)
	{
		return root + name + ".m3u";	
	}
	
	public PlaylistInfo(String name, String displayName, int trackCount) {
		this.name = name;
		this.displayName = displayName;
		this.trackCount = trackCount;
	}
	
	public static PlaylistInfo fromJson(JSONObject json) throws JSONException{
		return new PlaylistInfo(json.getString("Name"), json.getString("DisplayName"), json.getInt("TrackCount"));
	}
	
	public String toString()
	{
		StringBuffer sb = new StringBuffer();
		sb.append(displayName);
		sb.append(" (");
		sb.append(trackCount);
		sb.append(")");
		return sb.toString();
	}
}
