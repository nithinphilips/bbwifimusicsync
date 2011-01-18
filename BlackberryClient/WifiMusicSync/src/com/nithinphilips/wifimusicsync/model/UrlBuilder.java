package com.nithinphilips.wifimusicsync.model;

public class UrlBuilder {
	
	String serverRoot;
	
	public UrlBuilder(String serverRoot){
		if(serverRoot.endsWith("/"))
			this.serverRoot = serverRoot.substring(0, serverRoot.length() - 1);
		else
			this.serverRoot = serverRoot;
	}
	
	public String getRoot()
	{
		return serverRoot;
	}
	
	public String getFullUrl(String path)
	{
		return serverRoot + path;
	}
	
	public String getQueryUrl()
	{
		return serverRoot + "/query";
	}
	
	public String getSubscribe()
	{
		return serverRoot + "/subscribe";
	}
	
	public String getHelloUrl()
	{
		return serverRoot + "/hello";
	}
	
	public String getSongUrl(String songId)
	{
		return serverRoot + "/songs/" + songId;
	}
	
	public String getPlaylistUrl(String playlistId)
	{
		return serverRoot + "/playlists/" + playlistId;
	}
	
	public String getPlaylistsUrl()
	{
		return serverRoot + "/getplaylists";
	}
	
}
