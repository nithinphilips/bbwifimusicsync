package com.nithinphilips.wifimusicsync.components;

import java.util.Vector;

import com.nithinphilips.wifimusicsync.model.PlaylistInfo;
import com.nithinphilips.wifimusicsync.model.SyncAction;

import net.rim.device.api.collection.ReadableList;
import net.rim.device.api.system.Bitmap;
import net.rim.device.api.system.Characters;
import net.rim.device.api.system.Display;
import net.rim.device.api.ui.Color;
import net.rim.device.api.ui.DrawStyle;
import net.rim.device.api.ui.Font;
import net.rim.device.api.ui.FontManager;
import net.rim.device.api.ui.Graphics;
import net.rim.device.api.ui.MenuItem;
import net.rim.device.api.ui.component.ListField;
import net.rim.device.api.ui.component.ListFieldCallback;

/** custom listmodel that sync's and auto-binds with the listfield */
public class SyncItemsListModel implements ListFieldCallback, SyncActionChangedCallBack
{

    private ReadableList    _data;
    private ListField _view;
    private int       _defaultRowHeight = Font.getDefault().getHeight();
    private int       _defaultRowWidth  = 0;

    /** constructor that saves a ref to the model's view - {@link ListField}, and binds this model to the view */
    public SyncItemsListModel(ListField list, ReadableList data)
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

    /** list row renderer */
    public void drawListRow(ListField list, Graphics g, int index, int y, int w)
    {
        g.setColor(Color.WHITE);
    	StringBuffer rowString = new StringBuffer();
    	
        PlaylistInfo syncAction = (PlaylistInfo) _data.getAt(index);
        
        rowString.append(Characters.SPACE);
        if(syncAction.isSelected()){
        	rowString.append(Characters.BALLOT_BOX_WITH_CHECK);
        }else{
        	rowString.append(Characters.BALLOT_BOX);
        }
        rowString.append(Characters.SPACE);
        rowString.append(syncAction.getDisplayName());
        
        g.drawText(rowString.toString(), 0, y, 0, w);
    }

    /** list row data accessor */
    public Object get(ListField list, int index)
    {
        return _data.getAt(index);
    }

    /** used for filtering list elements */
    public int indexOfList(ListField list, String p, int s)
    {
        return _data.getIndex(p);
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
        //_data.addElement(toInsert);

        // update the view
        _view.insert(index);
        
        toInsert.setNotifier(this);
        toInsert.setIndex(index);
    }

    /** mutator, which syncs model and view */
    public void delete(int index)
    {
        // update the model
        //_data.removeElementAt(index);

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
//        _data.removeElementAt(index);
//        _data.insertElementAt(newValue, index);

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
