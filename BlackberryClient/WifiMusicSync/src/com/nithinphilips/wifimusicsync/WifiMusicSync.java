package com.nithinphilips.wifimusicsync;

import java.util.Vector;

import net.rim.device.api.ui.UiApplication;


public final class WifiMusicSync extends UiApplication{

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		
//		try {
//			System.out.println("---------------------------------------------");
//			PlaylistListResponse response = PlaylistListResponse.fromJson(new JSONObject("{\"Tracks\":[{\"Name\":\"Library\",\"TrackCount\":23437},{\"Name\":\"Neethu\",\"TrackCount\":95}]}"));
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
