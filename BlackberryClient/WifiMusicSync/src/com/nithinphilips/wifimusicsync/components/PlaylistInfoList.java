package com.nithinphilips.wifimusicsync.components;

import org.openqa.selenium.remote.me.util.StringUtils;

import com.nithinphilips.wifimusicsync.model.PlaylistInfo;

import net.rim.device.api.collection.util.UnsortedReadableList;
import net.rim.device.api.system.Characters;
import net.rim.device.api.ui.component.KeywordProvider;
import net.rim.device.api.util.StringUtilities;

public class PlaylistInfoList extends UnsortedReadableList implements KeywordProvider{
	
	public PlaylistInfoList()
    {
        super();        
    } 
   
    public void addElement(Object element)
    {
        doAdd(element);        
    }
    
	public String[] getKeywords(Object element) {
		if(element instanceof PlaylistInfo)
        {
            return StringUtils.split(((PlaylistInfo)element).getDisplayName(), Characters.SPACE);
        }
        return null;
	}
}
