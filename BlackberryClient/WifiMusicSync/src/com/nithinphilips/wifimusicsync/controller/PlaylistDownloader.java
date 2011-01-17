package com.nithinphilips.wifimusicsync.controller;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

import javax.microedition.io.Connector;
import javax.microedition.io.HttpConnection;
import javax.microedition.io.file.FileConnection;

import org.json.me.JSONException;
import org.json.me.JSONObject;

import com.nithinphilips.JsonHttpHelper;
import com.nithinphilips.wifimusicsync.WifiMusicSync;
import com.nithinphilips.wifimusicsync.model.PlaylistRequest;
import com.nithinphilips.wifimusicsync.model.SyncAction;
import com.nithinphilips.wifimusicsync.model.SyncResponse;
import com.nithinphilips.wifimusicsync.model.UrlBuilder;

public class PlaylistDownloader {

	UrlBuilder server;
	String playlistPath;
	String rootPath;
	String clientId;

	public PlaylistDownloader(String serverUrl, String playlistPath, String rootPath,
			String clientId) {
		this.server = new UrlBuilder(serverUrl);
		this.playlistPath = playlistPath;
		this.rootPath = rootPath;
		this.clientId = clientId;
	}

	public void syncPlaylist() throws JSONException, IOException{
		syncPlaylist(server, rootPath, playlistPath);
	}
	
	void syncPlaylist(UrlBuilder server, String root, String playlist)
			throws JSONException, IOException {

		String mediaRoot;
		if (root.endsWith("/"))
			mediaRoot = root;
		else
			mediaRoot = root + "/";

		PlaylistRequest playlistRequest = new PlaylistRequest();
		playlistRequest.setDeviceId(clientId);
		playlistRequest.setPlaylistDevicePath(playlist);
		playlistRequest.setDeviceMediaRoot(mediaRoot);
		playlistRequest.loadPlaylistData(playlist);

		String s_response = JsonHttpHelper.executeCommand(server.getQueryUrl(),
				playlistRequest.toJsonObject().toString());

		if ((s_response == null) || (s_response.compareTo("") == 0)) {
			log("Error: No response from server.");
		} else {
			SyncResponse response = SyncResponse.fromJson(new JSONObject(
					s_response), server);

			if (response.getError() == SyncResponse.ERROR_NONE) {
				// exec actions
				executeActions(response.getActions());

				// update playlist
				downloadFile(response.getPlaylistServerUrl(), response
						.getPlaylistDevicePath(), true);
			} else {
				log("Server Error " + response.getErrorMessage());
			}
		}
	}

	void executeActions(SyncAction[] actions) throws JSONException, IOException {
		for (int i = 0; i < actions.length; i++) {
			if (actions[i].getType() == SyncAction.ADD) {
				log("Add " + actions[i].getTrackUrl() + " "
						+ actions[i].getDeviceLocation());
				createDirectoryTree(actions[i].getDeviceLocation());
				downloadFile(actions[i].getTrackUrl(), actions[i]
						.getDeviceLocation());
			} else if (actions[i].getType() == SyncAction.REMOVE) {
				log("Delete " + actions[i].getDeviceLocation());
				deleteFile(actions[i].getDeviceLocation());
				// TODO: Delete empty directory trees too
			}
		}
	}

	private void log(String string) {

	}

	void deleteFile(String fileName) throws IOException {
		FileConnection fileConnection = null;
		try {
			fileConnection = (FileConnection) Connector.open(fileName);
			if (fileConnection.exists())
				fileConnection.delete();
		} finally {
			if (fileConnection != null) {
				try {
					fileConnection.close();
				} catch (Exception error) {
					/* log error */
				}
			}
		}
	}

	static final int BUFFER_SIZE = 1024;

	void createDirectoryTree(String fileName) throws IOException {
		// Create directories
		String[] segments = WifiMusicSync.getDirectories("file:///", fileName);
		for (int i = 0; i < segments.length - 1; i++) {
			log(segments[i]);
			FileConnection dir = (FileConnection) Connector.open(segments[i],
					Connector.READ_WRITE);
			if (!dir.exists())
				dir.mkdir();
		}
	}

	void downloadFile(String url, String fileName) throws IOException {
		downloadFile(url, fileName, false);
	}

	void downloadFile(final String url, final String fileName,
			boolean forceOverwrite) throws IOException {
		log("Downloading: " + fileName + " from: " + url);

		HttpConnection httpConnection = null;
		FileConnection fileConnection = null;

		InputStream httpInStream = null;
		OutputStream fileOutStream = null;

		try {
			httpConnection = (HttpConnection) Connector.open(url
					+ JsonHttpHelper.URL_SUFFIX);
			// HTTP Request
			httpConnection.setRequestMethod(HttpConnection.GET);
			httpConnection.setRequestProperty("Connection", "close");

			int status = httpConnection.getResponseCode();

			if (status == HttpConnection.HTTP_OK) {
				fileConnection = (FileConnection) Connector.open(fileName,
						Connector.READ_WRITE);

				if (fileConnection.exists()) {
					if (forceOverwrite) {
						fileConnection.delete();
					} else {
						try {
							long contentLength = Long.parseLong(httpConnection
									.getHeaderField("Content-Length"));
							log("Sever: " + Long.toString(contentLength)
									+ " Client: "
									+ Long.toString(fileConnection.fileSize()));
							if (contentLength != fileConnection.fileSize()) {
								fileConnection.delete();
							} else {
								// Same file. No skip download
								httpConnection.close();
								log("Skip download");
								return;
							}
						} catch (Exception ex) {
							// Delete the file, to be safe
							if (fileConnection.exists())
								fileConnection.delete();
						}
					}
				}

				fileConnection.create();

				log("Creating " + fileName);

				httpInStream = httpConnection.openInputStream();
				fileOutStream = fileConnection.openOutputStream();

				byte[] readBuf = new byte[BUFFER_SIZE];
				while (true) {
					int read = httpInStream.read(readBuf);
					// Dialog.alert("Read " + Integer.toString(read));
					if (read == -1)
						break;
					fileOutStream.write(readBuf, 0, read);
				}

				fileOutStream.flush();

				log("Done: " + fileName);
			}
		} finally {
			try {
				if (httpInStream != null)
					httpInStream.close();
			} catch (Exception error) {
			}

			try {
				if (fileOutStream != null)
					fileOutStream.close();
			} catch (Exception error) {
			}

			try {
				if (httpConnection != null)
					httpConnection.close();
			} catch (Exception error) {
			}

			try {
				if (fileConnection != null)
					fileConnection.close();
			} catch (Exception error) {
			}
		}
	}

}
