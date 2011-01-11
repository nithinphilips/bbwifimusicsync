package com.nithinphilips.wifimusicsync;

import java.io.IOException;
import java.util.Vector;

import org.json.me.JSONException;
import org.json.me.JSONObject;

import com.nithinphilips.wifimusicsync.model.PlaylistRequest;
import com.nithinphilips.wifimusicsync.model.SyncResponse;

import net.rim.device.api.ui.UiApplication;


public final class WifiMusicSync extends UiApplication{

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		
//		try {
//			System.out.println("---------------------------------------------");
//			SyncResponse response = SyncResponse.fromJson(new JSONObject("{\"Error\":0,\"ErrorMessage\":null,\"ServerId\":\"WiFiMusicSync 1.0\",\"PlaylistServerPath\":\"/playlists/df81e8fde0181a565e268ab3130a25720ec089c5\",\"PlaylistDevicePath\":\"file:///SDCard/Test.m3u\",\"Actions\":[{\"Type\":\"Add\",\"DeviceLocation\":\"file:///SDCard/Blackberry/music/WiFiSync/The A_Teens/Pop 'Til You Drop/01 - Floorfiller.mp3\",\"TrackPath\":\"/songs/8a6e93b78958ad17f52c0bb4c6d8025ad90e0d19\"},{\"Type\":\"Add\",\"DeviceLocation\":\"file:///SDCard/Blackberry/music/WiFiSync/Blitzen Trapper/Furr/03 - Furr.mp3\",\"TrackPath\":\"/songs/c84bde1dcd210e3d947a1c8dd9a837985f42a32e\"},{\"Type\":\"Add\",\"DeviceLocation\":\"file:///SDCard/Blackberry/music/WiFiSync/Tom Petty/Highway Companion/04 - Down South.mp3\",\"TrackPath\":\"/songs/998ea6c0156c96fd0546c24a5fad05625e605882\"}]}"), "http://192.168.0.104:9000");
//			System.out.println(response.toString());
//			System.out.println("---------------------------------------------");
//		} catch (JSONException e) {
//			// TODO Auto-generated catch blocke
//			e.printStackTrace();
//		}
		
		WifiMusicSync theApp = new WifiMusicSync();
		theApp.enterEventDispatcher();
	}
	
	public WifiMusicSync() {
		// display a new screen
		pushScreen(new WifiMusicSyncScreen());
	}
	
	public static Vector splitToVector(String inString, String delimeter) {
		Vector vec = new Vector();
		try {
			int indexA = 0;
			int indexB = inString.indexOf(delimeter);

			while (indexB != -1) {
				if (indexB > indexA) vec.addElement(new String(inString.substring(indexA, indexB)));
				indexA = indexB + delimeter.length();
				indexB = inString.indexOf(delimeter, indexA);
			}
			vec.addElement(new String(inString.substring(indexA, inString.length())));
		} catch (Exception e) {
			//sysout

		}
		return vec;
	}
	
	public static String[] split(String inString, String delimeter) {
		String[] retAr = new String[0];
		try {
			Vector vec = new Vector();
			int indexA = 0;
			int indexB = inString.indexOf(delimeter);

			while (indexB != -1) {
				if (indexB > indexA)
					vec.addElement(new String(inString.substring(indexA, indexB)));
				indexA = indexB + delimeter.length();
				indexB = inString.indexOf(delimeter, indexA);
			}
			vec.addElement(new String(inString.substring(indexA, inString
					.length())));
			retAr = new String[vec.size()];
			for (int i = 0; i < vec.size(); i++) {
				retAr[i] = vec.elementAt(i).toString();
			}
		} catch (Exception e) {
			//sysout

		}
		return retAr;
	}
	
	public static String[] getDirectories(String root, String path){
		String subStr = path.substring(root.length());
		String[] parts = split(subStr, "/");
		String[] result = new String[parts.length];
		
		String last = root;
		for (int i = 0; i < parts.length; i++) {
			result[i] = last + parts[i] + "/";
			last = result[i];
		}
		
		return result;
	}
}
