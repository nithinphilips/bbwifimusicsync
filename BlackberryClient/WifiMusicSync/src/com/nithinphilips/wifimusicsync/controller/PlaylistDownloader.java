package com.nithinphilips.wifimusicsync.controller;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

import javax.microedition.io.Connector;
import javax.microedition.io.HttpConnection;
import javax.microedition.io.file.FileConnection;

import org.json.me.*;

import com.nithinphilips.JsonHttpHelper;
import com.nithinphilips.wifimusicsync.WifiMusicSync;
import com.nithinphilips.wifimusicsync.model.PlaylistRequest;
import com.nithinphilips.wifimusicsync.model.SyncAction;
import com.nithinphilips.wifimusicsync.model.SyncResponse;
import com.nithinphilips.wifimusicsync.model.UrlBuilder;

public class PlaylistDownloader
{

    UrlBuilder server;
    String     playlistPath;
    String     rootPath;
    String     clientId;

    public PlaylistDownloader(String serverUrl, String playlistPath, String rootPath, String clientId)
    {
        this.server = new UrlBuilder(serverUrl);
        this.playlistPath = playlistPath;
        this.rootPath = rootPath;
        this.clientId = clientId;
    }

    SyncResponse response;

    public SyncResponse getResponse() throws IOException, JSONException
    {

        this.response = null;

        PlaylistRequest playlistRequest = new PlaylistRequest();
        playlistRequest.setDeviceId(this.clientId);
        playlistRequest.setPlaylistDevicePath(this.playlistPath);
        playlistRequest.setDeviceMediaRoot(this.rootPath);
        playlistRequest.loadPlaylistData(this.playlistPath);

        String s_response = JsonHttpHelper.executeCommand(server.getQueryUrl(), playlistRequest.toJsonObject().toString());

        if ((s_response == null) || (s_response.compareTo("") == 0))
        {
            log("Error: No response from server.");
        }
        else
        {
            SyncResponse response = SyncResponse.fromJson(new JSONObject(s_response), server);

            if (response.getError() == SyncResponse.ERROR_NONE)
            {
                this.response = response;
            }
            else
            {
                log("Server Error " + response.getErrorMessage());
            }
        }

        return this.response;
    }

    public void handleResponse() throws JSONException, IOException
    {
        if (this.response == null) getResponse(); // Try once
        if (this.response == null) return; // Some error

        // exec actions
        executeActions(response.getActions());

        // update playlist
        downloadFile(response.getPlaylistServerUrl(), response.getPlaylistDevicePath(), true, null);
    }

    public static void executeActions(SyncAction[] actions) throws JSONException, IOException
    {
        for (int i = 0; i < actions.length; i++)
        {
            if (actions[i].getType() == SyncAction.ADD)
            {
                log("Add " + actions[i].getTrackUrl() + " " + actions[i].getDeviceLocation());
                createDirectoryTree(actions[i].getDeviceLocation());
                actions[i].setStatus("Downloading...");
                downloadFile(actions[i].getTrackUrl(), actions[i].getDeviceLocation(), actions[i]);
            }
            else if (actions[i].getType() == SyncAction.REMOVE)
            {
                log("Delete " + actions[i].getDeviceLocation());
                actions[i].setStatus("Deleting...");
                deleteFile(actions[i].getDeviceLocation());
                actions[i].setStatus("Completed");
                // TODO: Delete empty directory trees too
            }
        }
    }

    private static void log(String string)
    {

    }

    static void deleteFile(String fileName) throws IOException
    {
        FileConnection fileConnection = null;
        try
        {
            fileConnection = (FileConnection) Connector.open(fileName);
            if (fileConnection.exists()) fileConnection.delete();
        }
        finally
        {
            if (fileConnection != null)
            {
                try
                {
                    fileConnection.close();
                }
                catch (Exception error)
                {
                    /* log error */
                }
            }
        }
    }

    static final int BUFFER_SIZE = 1024;

    public static void createDirectoryTree(String fileName) throws IOException
    {
        // Create directories
        String[] segments = WifiMusicSync.getDirectories("file:///", fileName);
        for (int i = 0; i < segments.length - 1; i++)
        {
            log(segments[i]);
            FileConnection dir = (FileConnection) Connector.open(segments[i], Connector.READ_WRITE);
            if (!dir.exists()) dir.mkdir();
            dir.close();
        }
    }

    static void downloadFile(String url, String fileName, SyncAction updateReceiver) throws IOException
    {
        downloadFile(url, fileName, false, updateReceiver);
    }

    static void downloadFile(final String url, final String fileName, boolean forceOverwrite, SyncAction updateReceiver) throws IOException
    {
        log("Downloading: " + fileName + " from: " + url);

        HttpConnection httpConnection = null;
        FileConnection fileConnection = null;

        InputStream httpInStream = null;
        OutputStream fileOutStream = null;

        try
        {
            httpConnection = (HttpConnection) Connector.open(url + JsonHttpHelper.URL_SUFFIX);
            httpConnection.setRequestMethod(HttpConnection.GET);
            httpConnection.setRequestProperty("Connection", "close");

            int status = httpConnection.getResponseCode();


            if (status == HttpConnection.HTTP_OK)
            {
                long contentLength = Long.parseLong(httpConnection.getHeaderField("Content-Length"));

                fileConnection = (FileConnection) Connector.open(fileName, Connector.READ_WRITE);

                if (fileConnection.exists())
                {
                    if (forceOverwrite)
                    {
                        fileConnection.delete();
                    }
                    else
                    {
                        try
                        {
                            log("Sever: " + Long.toString(contentLength) + " Client: " + Long.toString(fileConnection.fileSize()));
                            if (contentLength != fileConnection.fileSize())
                            {
                                // File size mismatch. Redownload.
                                fileConnection.delete();
                            }
                            else
                            {
                                // Same file. No skip download
                                httpConnection.close();
                                if (updateReceiver != null) updateReceiver.setStatus("Skip duplicate");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            // Delete the file, to be safe
                            if (fileConnection.exists()) fileConnection.delete();
                        }
                    }
                }

                fileConnection.create();

                httpInStream = httpConnection.openInputStream();
                fileOutStream = fileConnection.openOutputStream();

                long totalRead = 0;

                byte[] readBuf = new byte[BUFFER_SIZE];
                while (true)
                {
                    int read = httpInStream.read(readBuf);

                    totalRead += read;
                    if (read == -1) break;
                    fileOutStream.write(readBuf, 0, read);
                    if(updateReceiver != null) updateReceiver.setStatus(Integer.toString(calculatePercent(totalRead, contentLength)) + "% done");
                }

                fileOutStream.flush();

                if(updateReceiver != null) updateReceiver.setStatus("Complete");
            }
        }
        finally
        {
            try
            {
                if (httpInStream != null) httpInStream.close();
            }
            catch (Exception error)
            {
            }

            try
            {
                if (fileOutStream != null) fileOutStream.close();
            }
            catch (Exception error)
            {
            }

            try
            {
                if (httpConnection != null) httpConnection.close();
            }
            catch (Exception error)
            {
            }

            try
            {
                if (fileConnection != null) fileConnection.close();
            }
            catch (Exception error)
            {
            }
        }
    }

    public static int calculatePercent(long value, long total)
    {
        return calculatePercent((double)value, (double)total);
    }

    public static int calculatePercent(double value, double total)
    {
        return (int)((value / total) * 100.00);
    }

}
