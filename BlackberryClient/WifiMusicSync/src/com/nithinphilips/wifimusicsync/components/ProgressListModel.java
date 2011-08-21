package com.nithinphilips.wifimusicsync.components;

import java.util.Vector;

import net.rim.device.api.system.Display;
import net.rim.device.api.ui.Color;
import net.rim.device.api.ui.DrawStyle;
import net.rim.device.api.ui.Font;
import net.rim.device.api.ui.Graphics;
import net.rim.device.api.ui.component.ListField;
import net.rim.device.api.ui.component.ListFieldCallback;

import com.nithinphilips.wifimusicsync.model.SyncAction;

/** custom listmodel that sync's and auto-binds with the listfield */
public class ProgressListModel implements ListFieldCallback, SyncActionChangedCallBack
{

    private Vector    _data             = new Vector();
    private ListField _view;
    private int       _defaultRowHeight = Font.getDefault().getHeight() * 2 + 2;

    /** constructor that saves a ref to the model's view - {@link ListField}, and binds this model to the view */
    public ProgressListModel(ListField list, Vector data)
    {
        // save a ref to the list view
        _view = list;
        _data = data;

        // bind this model to the given view
        list.setCallback(this);

        // set the default row height
        _view.setRowHeight(_defaultRowHeight);

        _view.setSize(_data.size());
    }

    // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    // implement ListFieldCallback interface
    // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX


    public static final int COLOR_NORMAL_INDEX   = 0x00aaaaaa;
    public static final int COLOR_SELECTED_INDEX = 0x00dddddd;

    public static final int COLOR_ADD_BAR_FILL   = 0x0080ffa1;
    public static final int COLOR_REM_BAR_FILL   = 0x00ff8080;

    public static final int COLOR_NAME_TEXT      = Color.WHITE;
    public static final int COLOR_STATUS_TEXT    = Color.GRAY;
    public static final int COLOR_SELECTED_STATUS_TEXT    = 0x00dddddd;

    /** list row renderer */
    public void drawListRow(ListField list, Graphics g, int index, int y, int w)
    {

        int text_X = 9;
        int indexColor = COLOR_NORMAL_INDEX;
        int statusColor = COLOR_STATUS_TEXT;

        if (list.getSelectedIndex() == index)
        {
            indexColor = COLOR_SELECTED_INDEX;
            statusColor = COLOR_SELECTED_STATUS_TEXT;
        }

        g.setColor(indexColor);
        g.setFont(Font.getDefault().derive(0, _defaultRowHeight + 5));

        SyncAction syncAction = (SyncAction) _data.elementAt(index);
        String _indexStr = Integer.toString(syncAction.getIndex() + 1);

        if (_indexStr.length() <= 1) _indexStr = "0" + _indexStr;
        int x_offset = g.drawText(_indexStr, text_X - 1, y - 4);

        if (syncAction.getType() == SyncAction.ADD)
            g.setColor(COLOR_ADD_BAR_FILL);
        else
            g.setColor(COLOR_REM_BAR_FILL);

        g.fillRect(0, y, text_X - 2, _defaultRowHeight);

        if (index == 0)
        {
            g.setColor(indexColor);
            g.drawLine(0, y, w, y);
        }
        text_X += x_offset - 2;

        // File Name
        g.setColor(COLOR_NAME_TEXT);
        g.setFont(Font.getDefault());
        g.drawText(syncAction.getFileName(), text_X, y, DrawStyle.LEADING | DrawStyle.ELLIPSIS, w);

        // Status Line
        g.setColor(statusColor);
        g.setFont(Font.getDefault().derive(0, Font.getDefault().getHeight() - 3));
        g.drawText(syncAction.getStatus(), text_X, y + Font.getDefault().getHeight() + 2, DrawStyle.LEADING | DrawStyle.ELLIPSIS, w);

        // Bottom Line
        g.setColor(indexColor);
        g.drawLine(0, y + _defaultRowHeight - 1, w, y + _defaultRowHeight - 1);

    }

    /** list row data accessor */
    public Object get(ListField list, int index)
    {
        return _data.elementAt(index);
    }

    /** used for filtering list elements */
    public int indexOfList(ListField list, String p, int s)
    {
        return _data.indexOf(p, s);
    }

    /** used for rendering list... provide the width of the list in pixels */
    public int getPreferredWidth(ListField list)
    {
        return Display.getWidth();
    }

    // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    // data manipulation methods... not part of the interface
    // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

    public int insert(SyncAction toInsert)
    {
        int index = this.size();
        insert(toInsert, index);
        return index;
    }

    /** mutator, which syncs model and view */
    public void insert(SyncAction toInsert, int index)
    {
        // update the model
        _data.addElement(toInsert);

        // update the view
        _view.insert(index);

        toInsert.setNotifier(this);
        toInsert.setIndex(index);
    }

    /** mutator, which syncs model and view */
    public void delete(int index)
    {
        // update the model
        _data.removeElementAt(index);

        // update the view
        _view.delete(index);
    }

    /** mutator, which syncs model and view */
    public void erase()
    {
        int size = _data.size();

        // update the view
        for (int i = 0; i < size; i++)
        {
            delete(size - i - 1);
        }
    }

    public void modify(SyncAction newValue, int index)
    {
        // update the model
        _data.removeElementAt(index);
        _data.insertElementAt(newValue, index);

        // update the view
        _view.invalidate(index);
    }

    public int size()
    {
        return _data.size();
    }

    public void changed(SyncAction action)
    {
        _view.invalidate(action.getIndex());
    }

}
