package com.nithinphilips.wifimusicsync;

import net.rim.device.api.ui.Color;
import net.rim.device.api.ui.DrawStyle;
import net.rim.device.api.ui.Graphics;
import net.rim.device.api.ui.component.Dialog;
import net.rim.device.api.ui.component.KeywordFilterField;
import net.rim.device.api.ui.container.MainScreen;
import net.rim.device.api.ui.container.VerticalFieldManager;
import net.rim.device.api.ui.decor.BackgroundFactory;

import com.nithinphilips.wifimusicsync.components.PlaylistInfoList;
import com.nithinphilips.wifimusicsync.model.PlaylistInfo;

public class SyncItemsSelectionScreen extends MainScreen
{
    KeywordFilterField keywordFilterField;
    PlaylistInfoList   playlistInfos;

    int result = Dialog.CANCEL;

    public SyncItemsSelectionScreen(PlaylistInfo[] choices, String title)
    {

        playlistInfos = new PlaylistInfoList();
        for (int i = 0; i < choices.length; i++)
        {
            playlistInfos.addElement(choices[i]);
        }

        this.keywordFilterField = new KeywordFilterField() {

            public void paint(Graphics graphics)
            {
                graphics.setColor(Color.WHITE);
                super.paint(graphics);
            }

            protected boolean trackwheelUnclick(int status, int time)
            {
                // Get the index of the selected row.
                int index = getSelectedIndex();

                // Get the ChecklistData for this row.
                PlaylistInfo playlistInfo = (PlaylistInfo) keywordFilterField.getElementAt(index);

                // Toggle its status.
                playlistInfo.setSelected(!playlistInfo.isSelected());

                // Invalidate the modified row of the ListField.
                invalidate(index);

                setDirty(true);

                return true;
            }
        };

        this.keywordFilterField.setEmptyString("No items found.", DrawStyle.HCENTER);
        this.keywordFilterField.setLabel("Search " + title);
        this.keywordFilterField.setSourceList(playlistInfos, playlistInfos);

        ((VerticalFieldManager) getMainManager()).setBackground(BackgroundFactory.createSolidBackground(Color.BLACK));

        setTitle(keywordFilterField.getKeywordField());
        add(keywordFilterField);
    }

    public int getResult()
    {
        return result;
    }

    public void save()
    {
        result = Dialog.OK;
    }
}
