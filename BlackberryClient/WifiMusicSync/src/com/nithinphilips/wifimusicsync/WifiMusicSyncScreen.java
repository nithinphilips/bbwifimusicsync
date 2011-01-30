package com.nithinphilips.wifimusicsync;

import java.io.IOException;
import java.util.Enumeration;
import java.util.Vector;

import net.rim.device.api.command.CommandHandler;
import net.rim.device.api.command.ReadOnlyCommandMetadata;
import net.rim.device.api.system.Bitmap;
import net.rim.device.api.ui.Color;
import net.rim.device.api.ui.DrawStyle;
import net.rim.device.api.ui.Field;
import net.rim.device.api.ui.MenuItem;
import net.rim.device.api.ui.TransitionContext;
import net.rim.device.api.ui.Ui;
import net.rim.device.api.ui.UiApplication;
import net.rim.device.api.ui.UiEngineInstance;
import net.rim.device.api.ui.component.Dialog;
import net.rim.device.api.ui.component.ListField;
import net.rim.device.api.ui.component.StandardTitleBar;
import net.rim.device.api.ui.container.MainScreen;
import net.rim.device.api.ui.container.VerticalFieldManager;
import net.rim.device.api.ui.decor.BackgroundFactory;
import net.rim.device.api.ui.image.Image;
import net.rim.device.api.ui.image.ImageFactory;
import net.rim.device.api.ui.menu.CommandItem;
import net.rim.device.api.ui.menu.CommandItemProvider;
import net.rim.device.api.ui.menu.DefaultContextMenuProvider;
import net.rim.device.api.util.StringProvider;

import org.json.me.JSONException;

import com.fairview5.keepassbb2.common.ui.ProgressDialog;
import com.nithinphilips.wifimusicsync.components.PlaylistSelectionDialog;
import com.nithinphilips.wifimusicsync.components.ProgressListModel;
import com.nithinphilips.wifimusicsync.components.WifiMusicSyncProperties;
import com.nithinphilips.wifimusicsync.controller.PlaylistDownloader;
import com.nithinphilips.wifimusicsync.controller.Subscriber;
import com.nithinphilips.wifimusicsync.model.PlaylistInfo;
import com.nithinphilips.wifimusicsync.model.SyncAction;
import com.nithinphilips.wifimusicsync.model.SyncResponse;

public class WifiMusicSyncScreen extends MainScreen
{

    // ProgressView: http://docs.blackberry.com/en/developers/deliverables/17971/Indicate_progress_1210003_11.jsp
    // Please Wait Screen: http://supportforums.blackberry.com/t5/Java-Development/Sample-quot-Please-Wait-quot-screen-part-1/ta-p/493808

    ListField         myListView;
    ProgressListModel myListModel;

    public WifiMusicSyncScreen()
    {
        super(VERTICAL_SCROLL);

        ((VerticalFieldManager) getMainManager()).setBackground(BackgroundFactory.createSolidBackground(Color.BLACK));

        StandardTitleBar _titleBar = new StandardTitleBar();
        _titleBar.addTitle("Music Sync");
        _titleBar.addNotifications();
        _titleBar.addSignalIndicator();
        this.setTitle(_titleBar);

        this.myListView = new ListField();
        this.myListView.setEmptyString("Choose 'Sync' to start syncing.", DrawStyle.HCENTER);
        this.myListModel = new ProgressListModel(this.myListView, new Vector());

        add(this.myListView);

        setContextMenuProvider(new DefaultContextMenuProvider());

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

        final CommandHandler subscribeCommand = new CommandHandler() {
            public void execute(ReadOnlyCommandMetadata metadata, Object context)
            {
                subscribe();
            }
        };

        CommandItemProvider provider = new CommandItemProvider() {

            public Object getContext(Field field)
            {
                return field;
            }

            public Vector getItems(Field field)
            {
                Vector items = new Vector();

                // from <https://code.google.com/p/ultimate-gnome/> and Tango-Icon project
                Image syncIcon = ImageFactory.createImage(Bitmap.getBitmapResource("view-refresh.png"));
                items.addElement(new CommandItem(new StringProvider("Sync"), syncIcon, new net.rim.device.api.command.Command(syncCommand)));

                Image subscribeIcon = ImageFactory.createImage(Bitmap.getBitmapResource("gnome-app-install-star.png"));
                items.addElement(new CommandItem(new StringProvider("Choose Playlists"), subscribeIcon, new net.rim.device.api.command.Command(subscribeCommand)));

                return items;
            }
        };

        setCommandItemProvider(provider);

        MenuItem menuItemSync = new MenuItem(new StringProvider("Sync"), 10000000, 100);
        menuItemSync.setCommand(new net.rim.device.api.command.Command(syncCommand));

        MenuItem menuItemMakePlaylist = new MenuItem(new StringProvider("Choose Playlists"), 100000, 100);
        menuItemMakePlaylist.setCommand(new net.rim.device.api.command.Command(subscribeCommand));

        MenuItem menuItemOptions = new MenuItem(new StringProvider("Options"), 100000, 100);
        menuItemOptions.setCommand(new net.rim.device.api.command.Command(optionsCommand));

        MenuItem menuItemTest = new MenuItem(new StringProvider("Test"), 10000, 100) {
            public void run()
            {
                Thread t = new Thread() {
                    public void run()
                    {
                        final SyncAction addaction = new SyncAction();
                        final SyncAction remaction = new SyncAction();

                        addaction.setDeviceLocation("file:///SDCard/add.mp3");
                        addaction.setType(SyncAction.ADD);

                        remaction.setDeviceLocation("file:///SDCard/test2/remove.mp3");
                        remaction.setType(SyncAction.REMOVE);

                        addaction.setStatus("Queued");
                        remaction.setStatus("Queued");

                        UiApplication.getUiApplication().invokeLater(new Runnable() {
                            public void run()
                            {
                                addaction.setIndex(WifiMusicSyncScreen.this.myListModel.insert(addaction));
                                remaction.setIndex(WifiMusicSyncScreen.this.myListModel.insert(remaction));
                            }
                        });

                    }
                };
                t.start();
            }
        };

        addMenuItem(menuItemSync);
        addMenuItem(menuItemOptions);
        addMenuItem(menuItemMakePlaylist);
        // addMenuItem(menuItemTest);
    }

    private void log(final String message)
    {
        // UiApplication.getUiApplication().invokeLater(new Runnable() {
        // public void run() {
        // //Dialog.alert(message);
        // //statusField.setText(statusField.getText() + "\n" + message);
        // }
        // });
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

    void subscribe()
    {

        ProgressDialog.prepareModal("Getting playlists...");

        Thread t = new Thread() {
            public void run()
            {
                try
                {
                    final PlaylistInfo[] playlists;
                    final WifiMusicSyncProperties props = WifiMusicSyncProperties.fetch();

                    final Subscriber subscriber = new Subscriber(props.getServerUrl(), props.getLocalStoreRoot(), props.getClientId());

                    playlists = subscriber.getPlaylists();

                    ProgressDialog.closeProgress();

                    if (playlists != null) UiApplication.getUiApplication().invokeLater(new Runnable() {
                        public void run()
                        {
                            try
                            {
                                PlaylistSelectionDialog diag = new PlaylistSelectionDialog(playlists);
                                UiApplication.getUiApplication().pushModalScreen(diag);
                                if (diag.getResult() == Dialog.OK)
                                {
                                    // 1. Add newly checked
                                    // playlists
                                    for (int i = 0; i < playlists.length; i++)
                                        if (playlists[i].isSelected()) playlists[i].createOnFileSystem(props.getLocalStoreRoot());
                                        else playlists[i].deleteOnFileSystem(props.getLocalStoreRoot());

                                    // 2. A. Delete unchecked
                                    // playlists B. Cleanup
                                    // unreferenced tracks
                                    subscriber.updateSubscription();
                                }
                            }
                            catch (JSONException e)
                            {
                                log(e.toString());
                            }
                            catch (IOException e)
                            {
                                log(e.toString());
                            }
                        }
                    });
                    else
                    {

                    }

                }
                catch (JSONException e)
                {
                    log(e.toString());
                }
                catch (IOException e)
                {
                    log(e.toString());
                }

            }
        };
        t.start();
        ProgressDialog.doModal();

    }

    void sync()
    {

        ProgressDialog.prepareModal("Connecting to server...");
        Thread t = new Thread() {
            public void run()
            {
                try
                {
                    WifiMusicSyncProperties props = WifiMusicSyncProperties.fetch();
                    Enumeration playlists = Subscriber.findPlaylists(props.getLocalStoreRoot());
                    if (playlists != null) while (playlists.hasMoreElements())
                    {
                        String playlist = props.getLocalStoreRoot() + (String) playlists.nextElement();

                        PlaylistDownloader downloader = new PlaylistDownloader(props.getServerUrl(), playlist, props.getLocalStoreRoot(), props.getClientId());

                        SyncResponse response = downloader.getResponse();

                        ProgressDialog.closeProgress();

                        if (response != null)
                        {
                            final SyncAction[] actions = response.getActions();

                            UiApplication.getUiApplication().invokeLater(new Runnable() {
                                public void run()
                                {
                                    WifiMusicSyncScreen.this.myListModel.erase();

                                    for (int i = 0; i < actions.length; i++)
                                        actions[i].setIndex(WifiMusicSyncScreen.this.myListModel.insert(actions[i]));
                                }
                            });

                            downloader.handleResponse();
                        }

                    }
                    else log("No playlists found");
                }
                catch (JSONException e)
                {
                    // TODO Auto-generated catch block
                    log(e.toString());
                }
                catch (IOException e)
                {
                    log(e.toString());
                }
                catch (IllegalArgumentException e)
                {
                    log(e.toString());
                }
            }
        };
        t.start();
        ProgressDialog.doModal();
    }

}
