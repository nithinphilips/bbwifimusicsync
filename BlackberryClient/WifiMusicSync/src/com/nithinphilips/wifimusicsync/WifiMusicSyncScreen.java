package com.nithinphilips.wifimusicsync;

import java.util.Enumeration;
import java.util.Timer;
import java.util.TimerTask;
import java.util.Vector;

// #ifdef BlackBerry6.0.0
import net.rim.device.api.applicationcontrol.ApplicationPermissions;
import net.rim.device.api.applicationcontrol.ApplicationPermissionsManager;
import net.rim.device.api.command.CommandHandler;
import net.rim.device.api.command.ReadOnlyCommandMetadata; // #endif
import net.rim.device.api.system.ApplicationDescriptor;
import net.rim.device.api.system.Bitmap;
import net.rim.device.api.ui.Color;
import net.rim.device.api.ui.DrawStyle;
import net.rim.device.api.ui.Field;
import net.rim.device.api.ui.Graphics;
import net.rim.device.api.ui.MenuItem;
import net.rim.device.api.ui.TransitionContext;
import net.rim.device.api.ui.Ui;
import net.rim.device.api.ui.UiApplication;
import net.rim.device.api.ui.UiEngineInstance;
import net.rim.device.api.ui.component.Dialog;
import net.rim.device.api.ui.component.LabelField;
import net.rim.device.api.ui.component.ListField; // #ifdef BlackBerry6.0.0
import net.rim.device.api.ui.component.Menu;
import net.rim.device.api.ui.component.StandardTitleBar; // #endif
import net.rim.device.api.ui.component.progressindicator.ActivityIndicatorController;
import net.rim.device.api.ui.component.progressindicator.ActivityIndicatorModel;
import net.rim.device.api.ui.component.progressindicator.ActivityIndicatorView;
import net.rim.device.api.ui.container.HorizontalFieldManager;
import net.rim.device.api.ui.container.MainScreen;
import net.rim.device.api.ui.container.VerticalFieldManager;
import net.rim.device.api.ui.decor.BackgroundFactory;
import net.rim.device.api.ui.image.Image;
import net.rim.device.api.ui.image.ImageFactory; // #ifdef BlackBerry6.0.0
import net.rim.device.api.ui.menu.CommandItem;
import net.rim.device.api.ui.menu.CommandItemProvider;
import net.rim.device.api.ui.menu.DefaultContextMenuProvider;
import net.rim.device.api.ui.menu.SubMenu;
import net.rim.device.api.util.StringProvider; // #endif

import com.fairview5.keepassbb2.common.ui.ProgressDialog;
import com.nithinphilips.Debug;
import com.nithinphilips.wifimusicsync.components.ProgressListModel;
import com.nithinphilips.wifimusicsync.components.WifiMusicSyncProperties;
import com.nithinphilips.wifimusicsync.controller.PlaylistDownloader;
import com.nithinphilips.wifimusicsync.controller.Subscriber;
import com.nithinphilips.wifimusicsync.model.PlaylistInfo;
import com.nithinphilips.wifimusicsync.model.SyncAction;
import com.nithinphilips.wifimusicsync.model.SyncResponse;
import com.nithinphilips.wifimusicsync.permissions.MusicSyncPermissionReasonProvider;

public class WifiMusicSyncScreen extends MainScreen
{

    // ProgressView: http://docs.blackberry.com/en/developers/deliverables/17971/Indicate_progress_1210003_11.jsp
    // Please Wait Screen: http://supportforums.blackberry.com/t5/Java-Development/Sample-quot-Please-Wait-quot-screen-part-1/ta-p/493808

    ListField                         myListView;
    ProgressListModel                 myListModel;
    LabelField                        statusLabel;

    public static final int           SUBSCRIBE_PLAYLISTS = 0;
    public static final int           SUBSCRIBE_ARTISTS   = 1;
    public static final int           SUBSCRIBE_ALBUMS    = 2;

    final MenuItem                    menuItemSync;
    final SubMenu                     subMenuChoose;
    final MenuItem                    menuItemChoosePlaylists;
    final MenuItem                    menuItemChooseAlbums;
    final MenuItem                    menuItemChooseArtists;
    final MenuItem                    menuItemOptions;
    final MenuItem                    menuItemAbout;
    final MenuItem                    menuItemTest;

    final ActivityIndicatorView       view                = new ActivityIndicatorView(FIELD_VCENTER);
    final ActivityIndicatorModel      model               = new ActivityIndicatorModel();
    final ActivityIndicatorController controller          = new ActivityIndicatorController();
    
    HorizontalFieldManager statusContainer = new HorizontalFieldManager(Field.USE_ALL_WIDTH) {
        public void paint(Graphics graphics)
        {
            graphics.setColor(Color.WHITE);
            super.paint(graphics);
        }
    };

    public WifiMusicSyncScreen()
    {
        super(VERTICAL_SCROLL);
        
        checkPermissions();

        ((VerticalFieldManager) getMainManager()).setBackground(BackgroundFactory.createSolidBackground(Color.BLACK));

        StandardTitleBar _titleBar = new StandardTitleBar();
        _titleBar.addTitle("Music Sync");
        _titleBar.addNotifications();
        _titleBar.addSignalIndicator();
        this.setTitle(_titleBar);

        // this.setTitle(new LabelField("Music Sync"));

        this.myListView = new ListField();
        this.myListView.setEmptyString("Choose 'Sync' to start syncing.", DrawStyle.HCENTER);
        this.myListModel = new ProgressListModel(this.myListView, new Vector());
        
        view.setController(controller);
        view.setModel(model);

        controller.setModel(model);
        controller.setView(view);

        model.setController(controller);        

        // Define the indicator image and create a field from it 
        Bitmap bitmap = Bitmap.getBitmapResource("progress-spinner.png");
        view.createActivityImageField(bitmap, 5, Field.FIELD_LEFT);
        //view.createLabel("Progressing...");

        add(this.myListView);

        statusLabel = new LabelField("", LabelField.USE_ALL_WIDTH);
        //statusLabel.setBackground(BackgroundFactory.createSolidBackground(Color.BLACK));
        
       
        statusContainer.setBackground(BackgroundFactory.createSolidBackground(Color.BLACK));
        statusContainer.add(statusLabel);
        setStatus(statusContainer);

        setContextMenuProvider(new DefaultContextMenuProvider());
        CommandItemProvider provider = new CommandItemProvider() {

            public Object getContext(Field field)
            {
                return field;
            }

            public Vector getItems(Field field)
            {
                Vector items = new Vector();

                // from <https://code.google.com/p/ultimate-gnome/> and 
                Image syncIcon = ImageFactory.createImage(Bitmap.getBitmapResource("sync.png"));
                items.addElement(new CommandItem(new StringProvider("Sync"), syncIcon, new net.rim.device.api.command.Command(syncCommand)));

                Image subscribePlaylistsIcon = ImageFactory.createImage(Bitmap.getBitmapResource("choose-playlist.png"));
                items.addElement(new CommandItem(new StringProvider("Choose Playlists"), subscribePlaylistsIcon, new net.rim.device.api.command.Command(subscribePlaylistsCommand)));
                
                Image subscribeAlbumIcon = ImageFactory.createImage(Bitmap.getBitmapResource("choose-album.png"));
                items.addElement(new CommandItem(new StringProvider("Choose Albums"), subscribeAlbumIcon, new net.rim.device.api.command.Command(subscribeAlbumsCommand)));
                
                Image subscribeArtistIcon = ImageFactory.createImage(Bitmap.getBitmapResource("choose-artist.png"));
                items.addElement(new CommandItem(new StringProvider("Choose Artists"), subscribeArtistIcon, new net.rim.device.api.command.Command(subscribeArtistsCommand)));

                return items;
            }
        };

        setCommandItemProvider(provider);

        menuItemSync = new MenuItem(new StringProvider("Sync"), 10000000, 100);
        menuItemSync.setCommand(new net.rim.device.api.command.Command(syncCommand));
        
        menuItemChoosePlaylists = new MenuItem(new StringProvider("Playlists"), 100, 100);
        menuItemChoosePlaylists.setCommand(new net.rim.device.api.command.Command(subscribePlaylistsCommand));
        
        menuItemChooseAlbums = new MenuItem(new StringProvider("Albums"), 200, 100);
        menuItemChooseAlbums.setCommand(new net.rim.device.api.command.Command(subscribeAlbumsCommand));
        
        menuItemChooseArtists = new MenuItem(new StringProvider("Artists"), 300, 100);
        menuItemChooseArtists.setCommand(new net.rim.device.api.command.Command(subscribeArtistsCommand));
        
        subMenuChoose = new SubMenu(new MenuItem[]{ menuItemChoosePlaylists, menuItemChooseAlbums, menuItemChooseArtists }, "Sync Selection", 10000000, 100);

        menuItemOptions = new MenuItem(new StringProvider("Options"), 100000, 100);
        menuItemOptions.setCommand(new net.rim.device.api.command.Command(optionsCommand));

        menuItemAbout = new MenuItem(new StringProvider("About"), 100000, 100);
        menuItemAbout.setCommand(new net.rim.device.api.command.Command(showAboutCommand));

        menuItemTest = new MenuItem(new StringProvider("Test"), 10000, 100);
        menuItemTest.setCommand(new net.rim.device.api.command.Command(runTestCommand));
        

        // Uncomment below for OS5

        // menuItemSync = new MenuItem("Sync", 10000000, 100){
        // public void run()
        // {
        // sync();
        // }
        // };
        //
        // menuItemMakePlaylist = new MenuItem("Choose Playlists", 100000, 100){
        // public void run()
        // {
        // subscribe();
        // }
        // };
        //
        // menuItemOptions = new MenuItem("Options", 100000, 100){
        // public void run()
        // {
        // pushSettingsScreen();
        // }
        // };
        //
        // menuItemAbout = new MenuItem("About", 100000, 100){
        // public void run()
        // {
        // pushAboutScreen();
        // }
        // };
        //        
        // menuItemTest = new MenuItem("Test", 100000, 100){
        // public void run()
        // {
        // runTest();
        // }
        // };
    }
    
    protected void makeMenu( Menu menu, int instance )
    {
        menu.add(subMenuChoose);
        menu.add(menuItemSync);
        menu.add(menuItemAbout);
        menu.add(menuItemOptions);
        menu.add(menuItemTest);
        
        super.makeMenu(menu, instance);
    }
    
    final CommandHandler optionsCommand = new CommandHandler() {
        public void execute(ReadOnlyCommandMetadata metadata, Object context)
        {
            pushSettingsScreen();
        }
    };

    final CommandHandler syncCommand = new CommandHandler() {
        public void execute(ReadOnlyCommandMetadata metadata, Object context)
        {
            sync();
        }
    };

    final CommandHandler subscribePlaylistsCommand = new CommandHandler() {
        public void execute(ReadOnlyCommandMetadata metadata, Object context)
        {
            subscribe(SUBSCRIBE_PLAYLISTS);
        }
    };
    
    final CommandHandler subscribeArtistsCommand = new CommandHandler() {
        public void execute(ReadOnlyCommandMetadata metadata, Object context)
        {
            subscribe(SUBSCRIBE_ARTISTS);
        }
    };
    
    final CommandHandler subscribeAlbumsCommand = new CommandHandler() {
        public void execute(ReadOnlyCommandMetadata metadata, Object context)
        {
            subscribe(SUBSCRIBE_ALBUMS);
        }
    };

    final CommandHandler showAboutCommand = new CommandHandler() {
        public void execute(ReadOnlyCommandMetadata metadata, Object context)
        {
            pushAboutScreen();
        }
    };

    final CommandHandler runTestCommand = new CommandHandler() {
        public void execute(ReadOnlyCommandMetadata metadata, Object context)
        {
            runTest();
        }
    };

    void runTest()
    {
         PlaylistInfo[] choices = new PlaylistInfo[20];
              
         for (int i = 0; i < choices.length; i++)
         choices[i] = new PlaylistInfo("", "Test" + i, i);
              
         pushChoicesScreen(choices, "Hello");
        

//        Thread t = new Thread() {
//            public void run()
//            {
//                final SyncAction addaction = new SyncAction();
//                final SyncAction remaction = new SyncAction();
//
//                addaction.setDeviceLocation("file:///SDCard/add.mp3");
//                addaction.setType(SyncAction.ADD);
//
//                remaction.setDeviceLocation("file:///SDCard/test2/remove.mp3");
//                remaction.setType(SyncAction.REMOVE);
//
//                addaction.setStatus("Downloading");
//                remaction.setStatus("Queued");
//
//                UiApplication.getUiApplication().invokeLater(new Runnable() {
//                    public void run()
//                    {
//                        WifiMusicSyncScreen.this.myListModel.insert(addaction);
//                        WifiMusicSyncScreen.this.myListModel.insert(remaction);
//                    }
//                });
//
//                TimerTask task = new TimerTask() {
//
//                    public void run()
//                    {
//                        UiApplication.getUiApplication().invokeLater(new Runnable() {
//                            public void run()
//                            {
//                                addaction.setStatus("Completed");
//                                remaction.setStatus("Completed");
//                            }
//                        });
//                    }
//                };
//
//                Timer timer = new Timer();
//                timer.schedule(task, 10000);
//
//            }
//        };
//        t.start();
    }

    void pushSettingsScreen()
    {

        SettingsScreen screen = new SettingsScreen();

        TransitionContext transitionContextIn;
        TransitionContext transitionContextOut;
        UiEngineInstance engine = Ui.getUiEngineInstance();

        transitionContextIn = new TransitionContext(TransitionContext.TRANSITION_SLIDE);
        transitionContextIn.setIntAttribute(TransitionContext.ATTR_DURATION, 250);
        transitionContextIn.setIntAttribute(TransitionContext.ATTR_DIRECTION, TransitionContext.DIRECTION_UP);

        transitionContextOut = new TransitionContext(TransitionContext.TRANSITION_SLIDE);
        transitionContextOut.setIntAttribute(TransitionContext.ATTR_DURATION, 250);
        transitionContextOut.setIntAttribute(TransitionContext.ATTR_DIRECTION, TransitionContext.DIRECTION_DOWN);
        transitionContextOut.setIntAttribute(TransitionContext.ATTR_KIND, TransitionContext.KIND_OUT);

        engine.setTransition(null, screen, UiEngineInstance.TRIGGER_PUSH, transitionContextIn);
        engine.setTransition(screen, null, UiEngineInstance.TRIGGER_POP, transitionContextOut);

        UiApplication.getUiApplication().pushModalScreen(screen);
    }

    void pushAboutScreen()
    {

        AboutScreen screen = new AboutScreen();

        TransitionContext transitionContextIn;
        TransitionContext transitionContextOut;
        UiEngineInstance engine = Ui.getUiEngineInstance();

        transitionContextIn = new TransitionContext(TransitionContext.TRANSITION_SLIDE);
        transitionContextIn.setIntAttribute(TransitionContext.ATTR_DURATION, 300);
        transitionContextIn.setIntAttribute(TransitionContext.ATTR_DIRECTION, TransitionContext.DIRECTION_DOWN);
        transitionContextIn.setIntAttribute(TransitionContext.ATTR_KIND, TransitionContext.KIND_IN);

        transitionContextOut = new TransitionContext(TransitionContext.TRANSITION_SLIDE);
        transitionContextOut.setIntAttribute(TransitionContext.ATTR_DURATION, 300);
        transitionContextOut.setIntAttribute(TransitionContext.ATTR_DIRECTION, TransitionContext.DIRECTION_UP);
        transitionContextOut.setIntAttribute(TransitionContext.ATTR_KIND, TransitionContext.KIND_OUT);

        engine.setTransition(null, screen, UiEngineInstance.TRIGGER_PUSH, transitionContextIn);
        engine.setTransition(screen, null, UiEngineInstance.TRIGGER_POP, transitionContextOut);

        UiApplication.getUiApplication().pushModalScreen(screen);
    }

    boolean pushChoicesScreen(PlaylistInfo[] choices, String title)
    {
        SyncItemsSelectionScreen screen = new SyncItemsSelectionScreen(choices, title);

        TransitionContext transitionContextIn;
        TransitionContext transitionContextOut;
        UiEngineInstance engine = Ui.getUiEngineInstance();

        transitionContextIn = new TransitionContext(TransitionContext.TRANSITION_SLIDE);
        transitionContextIn.setIntAttribute(TransitionContext.ATTR_DURATION, 250);
        transitionContextIn.setIntAttribute(TransitionContext.ATTR_DIRECTION, TransitionContext.DIRECTION_LEFT);
        transitionContextIn.setIntAttribute(TransitionContext.ATTR_KIND, TransitionContext.KIND_IN);

        transitionContextOut = new TransitionContext(TransitionContext.TRANSITION_SLIDE);
        transitionContextOut.setIntAttribute(TransitionContext.ATTR_DURATION, 250);
        transitionContextOut.setIntAttribute(TransitionContext.ATTR_DIRECTION, TransitionContext.DIRECTION_RIGHT);
        transitionContextOut.setIntAttribute(TransitionContext.ATTR_KIND, TransitionContext.KIND_OUT);

        engine.setTransition(null, screen, UiEngineInstance.TRIGGER_PUSH, transitionContextIn);
        engine.setTransition(screen, null, UiEngineInstance.TRIGGER_POP, transitionContextOut);

        UiApplication.getUiApplication().pushModalScreen(screen);

        return screen.getResult() == Dialog.OK;
    }

    void subscribe(final int sourceType)
    {
        beginProgressTask();
        setStatusMessage("Connecting...");
        ProgressDialog.prepareModal("Connecting...");

        Thread t = new Thread() {
            public void run()
            {
                try
                {
                    final PlaylistInfo[] playlists;
                    final String title;
                    final WifiMusicSyncProperties props = WifiMusicSyncProperties.fetch();
                    final Subscriber subscriber = new Subscriber(props.getServerUrl(), props.getLocalStoreRoot(), props.getClientId());

                    if (sourceType == SUBSCRIBE_ALBUMS)
                    {
                        playlists = subscriber.getAlbums();
                        title = "Select Albums to Sync";
                    }
                    else if (sourceType == SUBSCRIBE_ARTISTS)
                    {
                        playlists = subscriber.getArtists();
                        title = "Select Artists to Sync";
                    }
                    else
                    {
                        playlists = subscriber.getPlaylists();
                        title = "Select Playlists to Sync";
                    }

                    ProgressDialog.closeProgress();

                    if (playlists != null)
                    {
                        UiApplication.getUiApplication().invokeLater(new Runnable() {
                            public void run()
                            {
                                try
                                {
                                    if (pushChoicesScreen(playlists, title))
                                    {
                                        for (int i = 0; i < playlists.length; i++)
                                        {
                                            if (playlists[i].isSelected()) playlists[i].createOnFileSystem(props.getLocalStoreRoot());
                                            else playlists[i].deleteOnFileSystem(props.getLocalStoreRoot());
                                        }

                                        subscriber.updateSubscription();
                                    }
                                }
                                catch (Exception e)
                                {
                                    if (Debug.DEBUG) setStatusMessage(e.toString() + e.getMessage());
                                    else setStatusMessage("Critical error. Subscription failed.");
                                }
                            }
                        });
                    }
                    else setStatusMessage("Error. No response from server.");
                }
                catch (Exception e)
                {
                    if (Debug.DEBUG) setStatusMessage(e.toString() + e.getMessage());
                    else setStatusMessage("Critical error. Subscription failed.");
                }
                finally
                {
                    resetStatusMessage();
                }
            }
        };
        t.start();
        ProgressDialog.doModal();
    }

    void sync()
    {
        beginProgressTask();
        setStatusMessage("Syncing...");
        this.myListView.setEmptyString("Sync in progress...", DrawStyle.HCENTER);
        ProgressDialog.prepareModal("Connecting to server...");

        Thread t = new Thread() {
            public void run()
            {
                try
                {
                    WifiMusicSyncProperties props = WifiMusicSyncProperties.fetch();
                    Vector playlists = Subscriber.findPlaylists(props.getLocalStoreRoot());
                    
                    if (playlists != null)
                    {
                        for (int i = 0; i < playlists.size(); i++)
                        {
                            String playlistName = (String) playlists.elementAt(i);
                            String playlist = props.getLocalStoreRoot() + playlistName;

                            ProgressDialog.closeProgress();

                            // NOTE: We assume that the extension is a dot + 3 chars
                            setStatusMessage("Syncing " + PlaylistInfo.getFriendlyPlaylistName(playlistName) + "...");

                            PlaylistDownloader downloader = new PlaylistDownloader(props.getServerUrl(), playlist, props.getLocalStoreRoot(), props.getClientId());
                            SyncResponse response = downloader.getResponse();

                            if (response != null)
                            {
                                final SyncAction[] actions = response.getActions();

                                UiApplication.getUiApplication().invokeLater(new Runnable() {
                                    public void run()
                                    {
                                        WifiMusicSyncScreen.this.myListModel.erase();

                                        for (int i = 0; i < actions.length; i++)
                                        {
                                            WifiMusicSyncScreen.this.myListModel.insert(actions[i]);
                                        }
                                    }
                                });

                                downloader.handleResponse();
                            }
                            else
                            {
                                setStatusMessage("Error Syncing " + playlistName);
                            }
                        }
                        setStatusMessage("Sync Complete");
                    }
                    else setStatusMessage("No playlists found");
                } 
                catch (Exception e)
                {
                    if (Debug.DEBUG) setStatusMessage(e.toString() + e.getMessage());
                    else setStatusMessage("Critical error. Sync failed.");

                    ProgressDialog.closeProgress();
                }
                finally
                {
                    resetStatusMessage();
                }
            }
        };
        t.start();
        ProgressDialog.doModal();
    }

    void resetStatusMessage()
    {
        TimerTask task = new TimerTask() {

            public void run()
            {
                UiApplication.getUiApplication().invokeLater(new Runnable() {
                    public void run()
                    {
                        setStatusMessage("");
                        myListView.setEmptyString("Choose 'Sync' to start syncing.", DrawStyle.HCENTER);
                        myListModel.erase();
                    }
                });
            }
        };

        Timer timer = new Timer();
        timer.schedule(task, 10000);
        endProgressTask();
    }
    
    void beginProgressTask()
    {
        if (UiApplication.getUiApplication().isEventThread())
        {
            statusContainer.deleteAll();
            statusContainer.add(view);
            statusContainer.add(statusLabel);
            return;
        }
        UiApplication.getUiApplication().invokeLater(new Runnable() {
            public void run()
            {
                statusContainer.deleteAll();
                statusContainer.add(view);
                statusContainer.add(statusLabel);
            }
        });
    }
    
    void endProgressTask()
    {
        if (UiApplication.getUiApplication().isEventThread())
        {
            statusContainer.deleteAll();
            statusContainer.add(statusLabel);
            return;
        }
        UiApplication.getUiApplication().invokeLater(new Runnable() {
            public void run()
            {
                statusContainer.deleteAll();
                statusContainer.add(statusLabel);
            }
        });
    }

    void setStatusMessage(final String status)
    {
        if (UiApplication.getUiApplication().isEventThread())
        {
            statusLabel.setText(status);
            return;
        }
        UiApplication.getUiApplication().invokeLater(new Runnable() {
            public void run()
            {
                statusLabel.setText(status);
            }
        });
    }
    
 
    private void checkPermissions()
    {
        // Capture the current state of permissions and check against the requirements
        ApplicationPermissionsManager apm = ApplicationPermissionsManager.getInstance();
        ApplicationPermissions original = apm.getApplicationPermissions();

        // Set up and attach a reason provider
        MusicSyncPermissionReasonProvider drp = new MusicSyncPermissionReasonProvider();
        apm.addReasonProvider(ApplicationDescriptor.currentApplicationDescriptor(), drp);

        if(original.getPermission(ApplicationPermissions.PERMISSION_FILE_API) == ApplicationPermissions.VALUE_ALLOW &&
           original.getPermission(ApplicationPermissions.PERMISSION_INTERNET) == ApplicationPermissions.VALUE_ALLOW &&
           original.getPermission(ApplicationPermissions.PERMISSION_WIFI) == ApplicationPermissions.VALUE_ALLOW &&
           original.getPermission(ApplicationPermissions.PERMISSION_DEVICE_SETTINGS) == ApplicationPermissions.VALUE_ALLOW &&
           original.getPermission(ApplicationPermissions.PERMISSION_CROSS_APPLICATION_COMMUNICATION) == ApplicationPermissions.VALUE_ALLOW)
        {
            // All of the necessary permissions are currently available
            return;
        }

        // Create a permission request for each of the permissions your application
        // needs. Note that you do not want to list all of the possible permission
        // values since that provides little value for the application or the user.  
        // Please only request the permissions needed for your application.
        ApplicationPermissions permRequest = new ApplicationPermissions();
        permRequest.addPermission(ApplicationPermissions.PERMISSION_FILE_API);
        permRequest.addPermission(ApplicationPermissions.PERMISSION_INTERNET);
        permRequest.addPermission(ApplicationPermissions.PERMISSION_WIFI);
        permRequest.addPermission(ApplicationPermissions.PERMISSION_DEVICE_SETTINGS);
        permRequest.addPermission(ApplicationPermissions.PERMISSION_CROSS_APPLICATION_COMMUNICATION);
        

        boolean acceptance = ApplicationPermissionsManager.getInstance().invokePermissionsRequest(permRequest);

        if(acceptance)
        {
            // User has accepted all of the permissions
            return;
        }
        else
        {
            // The user has only accepted some or none of the permissions 
            // requested. In this sample, we will not perform any additional 
            // actions based on this information. However, there are several 
            // scenarios where this information could be used. For example,
            // if the user denied networking capabilities then the application 
            // could disable that functionality if it was not core to the 
            // operation of the application.
        }
    }


}
