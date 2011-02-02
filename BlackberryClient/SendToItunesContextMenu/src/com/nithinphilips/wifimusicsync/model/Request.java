package com.nithinphilips.wifimusicsync.model;


import org.json.me.JSONException;
import org.json.me.JSONObject;

public abstract class Request {
	
	protected String deviceId = null;
	String deviceMediaRoot = null;
	
	public String getDeviceId() {
		return deviceId;
	}
	
	public void setDeviceId(String deviceId) {
		this.deviceId = deviceId;
	}

	public String getDeviceMediaRoot() {
		return deviceMediaRoot;
	}
	
	public void setDeviceMediaRoot(String deviceMediaRoot) {
		this.deviceMediaRoot = deviceMediaRoot;
	}
	
	public abstract JSONObject toJsonObject() throws JSONException;
	
}
