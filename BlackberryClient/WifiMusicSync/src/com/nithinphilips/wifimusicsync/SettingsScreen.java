package com.nithinphilips.wifimusicsync;

import com.nithinphilips.*;
import com.nithinphilips.wifimusicsync.components.WifiMusicSyncProperties;

import net.rim.device.api.system.WLANInfo;
import net.rim.device.api.ui.*;
import net.rim.device.api.ui.component.*;
import net.rim.device.api.ui.container.*;
import net.rim.device.api.ui.decor.*;
import net.rim.device.api.ui.picker.FilePicker;
import net.rim.device.api.ui.picker.FilePicker.Listener;

public class SettingsScreen extends MainScreen{

	private EditField serverIpEdit;
    private EditField serverPortEdit;
    private EditField localStoreRoot;
    private EditField homeWifiEdit;
 
    private WifiMusicSyncProperties optionProperties; 
    
    public SettingsScreen(){
    	super(VERTICAL_SCROLL);
		
		StandardTitleBar _titleBar = new StandardTitleBar();
		_titleBar.addTitle("Music Sync Settings");
		_titleBar.addNotifications();
		_titleBar.addSignalIndicator();
		this.setTitle(_titleBar);
		
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
        localStoreRoot = UiFactory.createEditField(optionProperties.getLocalStoreRoot(), 0);        
        homeWifiEdit = UiFactory.createEditField(optionProperties.getHomeWifiName(), 0);

        
        VerticalFieldManager serverGroup = UiFactory.createVerticalFieldGroup("Server Settings");
        GridFieldManager serverGrid = new GridFieldManager(2, 2, GridFieldManager.AUTO_SIZE);
        // Make sure the EditFields stay within bounds
        serverGrid.setColumnProperty(1, GridFieldManager.AUTO_SIZE, 100);
        
        serverGrid.add(new LabelField("IP:"), GridFieldManager.FIELD_RIGHT | GridFieldManager.NON_FOCUSABLE);
        serverGrid.add(serverIpEdit);
        serverGrid.add(new LabelField("Port:"), GridFieldManager.FIELD_RIGHT | GridFieldManager.NON_FOCUSABLE);
        serverGrid.add(serverPortEdit);
        serverGroup.add(serverGrid);

//        ButtonField pickDirectoryButton = new ButtonField("Browse...", ButtonField.FIELD_RIGHT);
//        pickDirectoryButton.setChangeListener(new FieldChangeListener() {
//			public void fieldChanged(Field field, int context) {
//				FilePicker picker = FilePicker.getInstance();
//				picker.setPath(localStoreRoot.getText());
//				picker.setView(FilePicker.VIEW_ALL);
//				picker.setListener(new Listener() {
//					public void selectionDone(String selected) {
//						localStoreRoot.setText(selected);
//					}
//				});
//				picker.show();
//			}
//		});
        
        VerticalFieldManager storageGroup = UiFactory.createVerticalFieldGroup("Music Storage");
        storageGroup.add(new LabelField("Directory:"));
        storageGroup.add(localStoreRoot);
//        storageGroup.add(pickDirectoryButton);
        
        ButtonField getCurrentSsidButton = new ButtonField("Use Current SSID", ButtonField.FIELD_RIGHT);
        getCurrentSsidButton.setChangeListener(new FieldChangeListener() {
			public void fieldChanged(Field field, int context) {
				WLANInfo.WLANAPInfo apInfo = WLANInfo.getAPInfo();
				if(apInfo != null){
					homeWifiEdit.setText(apInfo.getSSID());
				}
			}
		});
        
        VerticalFieldManager wifiGroup = UiFactory.createVerticalFieldGroup("WiFi Settings");
        wifiGroup.add(new LabelField("Your Home WiFi SSID:"));
        wifiGroup.add(homeWifiEdit);
        wifiGroup.add(getCurrentSsidButton);
        
        mainScreen.add(serverGroup);
        mainScreen.add(storageGroup);
        mainScreen.add(wifiGroup);
    }

    public void save()
    {
        //Get the new values from the UI controls
        //and set them in optionProperties.
        optionProperties.setServerIp(serverIpEdit.getText());
        optionProperties.setServerPort(serverPortEdit.getText());
        optionProperties.setLocalStoreRoot(localStoreRoot.getText());
        optionProperties.setHomeWifiName(homeWifiEdit.getText());
 
        
        //Write our changes back to the persistent store.
        optionProperties.save();

        optionProperties = null;
    }
}
