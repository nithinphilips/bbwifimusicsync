package com.nithinphilips.wifimusicsync;

import java.util.Enumeration;

import javax.microedition.io.file.FileSystemRegistry;

import net.rim.device.api.system.DeviceInfo;
import net.rim.device.api.system.WLANInfo;
import net.rim.device.api.ui.Field;
import net.rim.device.api.ui.FieldChangeListener;
import net.rim.device.api.ui.Manager;
import net.rim.device.api.ui.component.ButtonField;
import net.rim.device.api.ui.component.ChoiceField;
import net.rim.device.api.ui.component.EditField;
import net.rim.device.api.ui.component.LabelField;
import net.rim.device.api.ui.component.ObjectChoiceField;
import net.rim.device.api.ui.component.StandardTitleBar;
import net.rim.device.api.ui.container.GridFieldManager;
import net.rim.device.api.ui.container.MainScreen;
import net.rim.device.api.ui.container.VerticalFieldManager;
import net.rim.device.api.ui.decor.BackgroundFactory;

import com.nithinphilips.UiFactory;
import com.nithinphilips.wifimusicsync.components.WifiMusicSyncProperties;

public class SettingsScreen extends MainScreen{

	private EditField serverIpEdit;
    private EditField serverPortEdit;
    private ObjectChoiceField localStoreType;
 
    private WifiMusicSyncProperties optionProperties; 
    
    private String sdCardLabel = "SD Card";
    private String deviceStoreLabel = "Device Memory";
    
    public SettingsScreen(){
    	super(VERTICAL_SCROLL);
		

        StandardTitleBar _titleBar = new StandardTitleBar();
        _titleBar.addTitle("Settings");
        _titleBar.addNotifications();
        _titleBar.addSignalIndicator();
        this.setTitle(_titleBar);
        
        //this.setTitle(new LabelField("Settings")); 
		
		((VerticalFieldManager) getMainManager()).setBackground(BackgroundFactory.createSolidBackground(UiFactory.COLOR_SCREEN_BACKGROUND));

    	populateMainScreen(this);
    	serverIpEdit.setFocus();
    }
    
    
    public void populateMainScreen(Manager mainScreen)
    {
    	mainScreen.setBackground(BackgroundFactory.createSolidBackground(UiFactory.COLOR_SCREEN_BACKGROUND));
    	
        //Read in the properties from the persistent store.
        optionProperties = WifiMusicSyncProperties.fetch();

        //Get the current values from optionProperties and create
        //the controls to represent them.
        
        serverIpEdit = UiFactory.createEditField(optionProperties.getServerIp(), 0);
        serverPortEdit = UiFactory.createNumericEditField(optionProperties.getServerPort());
        // 0 = SDCard, 1 = Device store
        // TODO: No sdcard option if no sdcard inserted.
        
        boolean hasSDCard = false;
        String root = null;
        Enumeration e = FileSystemRegistry.listRoots();
        while (e.hasMoreElements()) {
             root = (String) e.nextElement();
             if( root.equalsIgnoreCase("sdcard/") ) {
                 hasSDCard = true;
                 break;
             }
        }
        
        if(hasSDCard)
            localStoreType = new ObjectChoiceField("", new String[]{ deviceStoreLabel, sdCardLabel}, optionProperties.getLocalStoreType(), Field.FIELD_LEFT);
        else
            localStoreType = new ObjectChoiceField("", new String[]{ deviceStoreLabel }, 0, Field.USE_ALL_WIDTH);
        
        if((!hasSDCard) &&  optionProperties.getLocalStoreType() == WifiMusicSyncProperties.LOCAL_STORE_TYPE_SDCARD)
            localStoreType.setDirty(true);

        
        VerticalFieldManager serverGroup = UiFactory.createVerticalFieldGroup("Server Settings");
        GridFieldManager serverGrid = new GridFieldManager(2, 2, GridFieldManager.AUTO_SIZE);
        // Make sure the EditFields stay within bounds
        serverGrid.setColumnProperty(1, GridFieldManager.AUTO_SIZE, 100);
        
        serverGrid.add(new LabelField("IP:"), GridFieldManager.FIELD_RIGHT | GridFieldManager.NON_FOCUSABLE);
        serverGrid.add(serverIpEdit);
        serverGrid.add(new LabelField("Port:"), GridFieldManager.FIELD_RIGHT | GridFieldManager.NON_FOCUSABLE);
        serverGrid.add(serverPortEdit);
        serverGroup.add(serverGrid);

        
        VerticalFieldManager storageGroup = UiFactory.createVerticalFieldGroup("Media Storage");
        storageGroup.add(new LabelField("Select the location to store downloaded media:"));
        storageGroup.add(localStoreType);
        
        VerticalFieldManager deviceIdGroup = UiFactory.createVerticalFieldGroup("Device ID");
        deviceIdGroup.add(new LabelField("Sync Client ID: " + optionProperties.getClientId()));
        deviceIdGroup.add(new LabelField("This ID uniquely identifies your device. You should only add known and trusted devices to your server's allowed clients list."));
        
        mainScreen.add(serverGroup);
        mainScreen.add(storageGroup);
        mainScreen.add(deviceIdGroup);
    }

    public void save()
    {
        String storageChoice = (String)localStoreType.getChoice(localStoreType.getSelectedIndex());
        //Get the new values from the UI controls
        //and set them in optionProperties.
        optionProperties.setServerIp(serverIpEdit.getText());
        optionProperties.setServerPort(serverPortEdit.getText());
        
        if(storageChoice.equals(deviceStoreLabel))
            optionProperties.setLocalStoreRoot(WifiMusicSyncProperties.LOCAL_STORE_MEMORY_PATH);
        else if(storageChoice.equals(sdCardLabel))
            optionProperties.setLocalStoreRoot(WifiMusicSyncProperties.LOCAL_STORE_SD_PATH);
 
        
        //Write our changes back to the persistent store.
        optionProperties.save();

        optionProperties = null;
    }
}
