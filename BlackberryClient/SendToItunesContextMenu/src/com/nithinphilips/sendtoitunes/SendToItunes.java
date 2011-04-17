package com.nithinphilips.sendtoitunes;

import java.util.Vector;

import javax.microedition.content.*;

import org.json.me.JSONException;

import com.nithinphilips.JsonHttpHelper;
import com.nithinphilips.wifimusicsync.components.WifiMusicSyncProperties;
import com.nithinphilips.wifimusicsync.model.PlaylistRequest;
import com.nithinphilips.wifimusicsync.model.UrlBuilder;

import net.rim.blackberry.api.menuitem.ApplicationMenuItem;
import net.rim.blackberry.api.menuitem.ApplicationMenuItemRepository;
import net.rim.device.api.system.ApplicationDescriptor;
import net.rim.device.api.ui.*;
import net.rim.device.api.ui.component.*;

public final class SendToItunes extends UiApplication implements RequestListener
{    
    private static String ID = "com.nithinphilips.sendtoitunes"; 
    private static String CLASSNAME = "com.nithinphilips.sendtoitunes.SendToItunes";     
    
    /**
     * Entry point for application
     * @param args Command line arguments
     */
    public static void main(String[] args)
    {
        if(args != null && args.length > 0)
        {
            if (args[0].equals("startup"))
            {
                // Register this application as a content handler on startup
                register();
            }
        }
        else
        {
            // Create a new instance of the application and make the currently
            // running thread the application's event dispatch thread.
            SendToItunes app = new SendToItunes();
            app.enterEventDispatcher();        
        }       
    }       
    
    
    /**
     * Registers this application as a content handler for image, video,
     * and audio files
     */
    private static void register()
    {
        String[] types = {"audio/mp4", "audio/amr", "audio/mpeg"};
        String[] suffixes = {".mp4", ".m4A", ".amr", ".mp3"};                             
                    
        String[] actions = {ContentHandler.ACTION_SEND};  
        String[] actionNames = {"To iTunes"}; 
        ActionNameMap[] actionNameMaps = {new ActionNameMap(actions,actionNames,"en")};
        
        // Get access to the registry
        Registry registry = Registry.getRegistry(CLASSNAME); 
        
        ApplicationMenuItem appMenu = new ApplicationMenuItem(100) {
            
            public String toString()
            {
                return "Play in iTunes";
            }
            
            public Object run(Object context)
            {
                Dialog.alert(context.getClass().getName());
                return context;
            }
        };        

        try
        {                       
            // Register as a content handler       
            registry.register(CLASSNAME, types, suffixes, actions, actionNameMaps, ID, null);      
            
            ApplicationMenuItemRepository amir = ApplicationMenuItemRepository.getInstance();
            ApplicationDescriptor ad_startup = ApplicationDescriptor.currentApplicationDescriptor();
            ApplicationDescriptor ad_gui = new ApplicationDescriptor(ad_startup, "", null);
            amir.addMenuItem(ApplicationMenuItemRepository.MENUITEM_MUSIC_SERVICE_ITEM, appMenu, ad_gui);
            amir.addMenuItem(ApplicationMenuItemRepository.MENUITEM_MUSIC_SERVICE_LIST_ITEM, appMenu, ad_gui);
        }
        catch (ContentHandlerException che)
        {   
            System.out.println("Registry#register() threw " + che.toString());            
        }
        catch (ClassNotFoundException cnfe)
        {        
            System.out.println("Registry#register() threw " + cnfe.toString());
        }               
    }  
    
    
    /**
     * Creates a new SendMediaDemo object
     */
    public SendToItunes()
    {      
        try
        {
            // Get access to the ContentHandlerServer for this application and
            // register as a listener.
            ContentHandlerServer contentHandlerServer = Registry.getServer(CLASSNAME);
            contentHandlerServer.setListener(this);            
        }
        catch(ContentHandlerException che)
        {
            errorDialog("Registry.getServer(String) threw " + che.toString());
        } 
    }
    
    
    /**
     * RequestListener implementation
     * @param server The content handler server from which to request Invocation objects
     * 
     * @see javax.microedition.content.RequestListener#invocationRequestNotify(ContentHandlerServer)
     */
    public void invocationRequestNotify(ContentHandlerServer server) 
    {                   
        Invocation invoc = server.getRequest(false);     
        if(invoc != null)
        {
            String type = invoc.getType();
            final String url = invoc.getURL();
            
            if (type.equals("audio/mp4") || type.equals("audio/mpeg"))
            {

                Thread t = new Thread() {
                    public void run()
                    {
                        Vector playlistData = new Vector();
                        playlistData.addElement(url);
                        
                        WifiMusicSyncProperties props = WifiMusicSyncProperties.fetch();

                        PlaylistRequest request = new PlaylistRequest();
                        request.setDeviceId(props.getClientId());
                        request.setPlaylistDevicePath("");
                        request.setDeviceMediaRoot(props.getLocalStoreRoot());
                        request.setPlaylistData(playlistData);

                        try
                        {
                            // Dialog.alert(request.toJsonObject().toString());
                            JsonHttpHelper.executeCommand(new UrlBuilder(props.getServerUrl()).getPlaySongUrl(), request.toJsonObject().toString());
                        }
                        catch (JSONException e)
                        {
                            e.printStackTrace();
                        }
                        finally
                        {
                            System.exit(0);
                        }
                    }
                };
                t.start();
            }

            server.finish(invoc, Invocation.OK);
        }        
    }
    
    
    /**
     * Presents a dialog to the user with a given message
     * @param message The text to display
     */
    public static void errorDialog(final String message)
    {
        UiApplication.getUiApplication().invokeLater(new Runnable()
        {
            public void run()
            {
                Dialog.alert(message);
            } 
        });
    }
} 
