package com.nithinphilips.wifimusicsync.model;

import java.io.IOException;

import javax.microedition.io.Connector;
import javax.microedition.io.file.FileConnection;

import net.rim.device.api.system.Characters;

import org.json.me.JSONException;
import org.json.me.JSONObject;

import com.nithinphilips.wifimusicsync.controller.PlaylistDownloader;

public class PlaylistInfo
{

    String  name;
    String  displayName;
    String  extension = ".m3u";
    int     trackCount;
    boolean selected;

    public boolean isSelected()
    {
        return selected;
    }

    public void setSelected(boolean selected)
    {
        this.selected = selected;
    }

    public String getDisplayName()
    {
        return displayName;
    }

    public String getExtension()
    {
        return extension;
    }

    public String getName()
    {
        return name;
    }

    public int getTrackCount()
    {
        return trackCount;
    }

    public void setExtension(String extension)
    {
        this.extension = extension;
    }

    public boolean existsOnFileSystem(String root) throws IOException
    {
        FileConnection file = null;
        try
        {
            file = (FileConnection) Connector.open(getPath(root), Connector.READ);
            return file.exists();
        }/*catch(Exception ex){
            return false;
        }
        */
        finally
        {
            if(file != null) file.close();
        }
    }

    public void createOnFileSystem(String root) throws IOException
    {
        String path = getPath(root);
        PlaylistDownloader.createDirectoryTree(path);

        FileConnection file = (FileConnection) Connector.open(path, Connector.READ_WRITE);
        if (!file.exists()) file.create();

        file.close();
    }

    public void deleteOnFileSystem(String root) throws IOException
    {
        String path = getPath(root);
        PlaylistDownloader.createDirectoryTree(path);

        FileConnection file = (FileConnection) Connector.open(path, Connector.READ_WRITE);
        if (file.exists()) file.delete();

        file.close();
    }

    String getPath(String root)
    {
        return root + name + extension;
    }

    public PlaylistInfo(String name, String displayName, int trackCount)
    {
        this.name = name;
        this.displayName = displayName;
        this.trackCount = trackCount;
    }
    
    public static String getFriendlyPlaylistName(String name)
    {
        String _name = name.substring(0, name.length() - 4);
        if(_name.length() > 3)
        {
            String prefix = _name.substring(0, 3);
            if(prefix.equals("Al_"))
                return "album " + _name.substring(3);
            else if(prefix.equals("Ar_"))
                return "artist " + _name.substring(3);
        }

        return _name;
        
    }

    public static PlaylistInfo fromJson(JSONObject json) throws JSONException
    {
        return new PlaylistInfo(json.getString("Name"), json.getString("DisplayName"), json.getInt("TrackCount"));
    }

    public String toString()
    {
        StringBuffer sb = new StringBuffer();
        sb.append(Characters.SPACE);
        if(isSelected())
            sb.append(Characters.BALLOT_BOX_WITH_CHECK);
        else
            sb.append(Characters.BALLOT_BOX);
        sb.append(Characters.SPACE);
        
        sb.append(displayName);
        sb.append(" (");
        sb.append(trackCount);
        sb.append(")");
        return sb.toString();
    }
}
