package com.nithinphilips.wifimusicsync.components;

import java.util.Vector;

import com.nithinphilips.wifimusicsync.model.SyncAction;

import net.rim.device.api.system.Bitmap;
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
public class ProgressListModel implements ListFieldCallback, SyncActionChangedCallBack {

  private Vector _data = new Vector();
  private ListField _view;
  private int _defaultRowHeight = Font.getDefault().getHeight() * 2 + 2;
  private int _defaultRowWidth = _defaultRowHeight;

  /** constructor that saves a ref to the model's view - {@link ListField}, and binds this model to the view */
  public ProgressListModel (ListField list, Vector data) {
    // save a ref to the list view
    _view = list;
    _data = data;

    // bind this model to the given view
    list.setCallback(this);

    // set the default row height
    _view.setRowHeight(_defaultRowHeight);
    
    _view.setSize(_data.size());
  }

  //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
  // implement ListFieldCallback interface
  //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

  /** list row renderer */
  public void drawListRow(ListField list, Graphics g, int index, int y, int w) {

	int text_X = 14;
	  
	SyncAction task = (SyncAction) _data.elementAt(index);
	
	if(task.getType() == SyncAction.ADD){
		g.setColor(0x004D8C54);
		g.fillRect(0, y, text_X - 2, _defaultRowHeight);
	}else if(task.getType() == SyncAction.REMOVE){
		g.setColor(0x008A1C33);
		g.fillRect(0, y, text_X - 2, _defaultRowHeight);
	}
	
	g.setColor(Color.BLACK);
    // draw the text /w ellipsis if it's too long...
    g.drawText(task.getFileName(), text_X + 3, y, DrawStyle.LEADING | DrawStyle.ELLIPSIS,
               w - _defaultRowWidth);
    
    g.setColor(Color.GRAY);
    g.setFont(Font.getDefault().derive(Font.ITALIC, Font.getDefault().getHeight() - 3));
    
    g.drawText(task.getStatus(), text_X + 3, y + Font.getDefault().getHeight() + 2, DrawStyle.LEADING | DrawStyle.ELLIPSIS,
            w - _defaultRowWidth);

    // draw the to the left of the text...
    //g.drawBitmap(0, y, _bitmap.getWidth(), _bitmap.getHeight(), _bitmap, 0, 0);

  }

  /** list row data accessor */
  public Object get(ListField list, int index) {
    return _data.elementAt(index);
  }

  /** used for filtering list elements */
  public int indexOfList(ListField list, String p, int s) {
    return _data.indexOf(p, s);
  }

  /** used for rendering list... provide the width of the list in pixels */
  public int getPreferredWidth(ListField list) {
    return Display.getWidth();
  }

  //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
  // data manipulation methods...  not part of the interface
  //XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

  public void insert(SyncAction toInsert) {
	  insert(toInsert, this.size());
  }
  
  /** mutator, which syncs model and view */
  public void insert(SyncAction toInsert, int index) {
    // update the model
    _data.addElement(toInsert);

    // update the view
    _view.insert(index);
  }
  /** mutator, which syncs model and view */
  public void delete(int index) {
    // update the model
    _data.removeElementAt(index);

    // update the view
    _view.delete(index);
  }
  /** mutator, which syncs model and view */
  public void erase() {
    int size = _data.size();

    // update the view
    for (int i = 0; i < size; i++) {
      delete(size - i - 1);
    }
  }
  public void modify(SyncAction newValue, int index) {
    // update the model
    _data.removeElementAt(index);
    _data.insertElementAt(newValue, index);

    // update the view
    _view.invalidate(index);
  }
  
  public int size() {
    return _data.size();
  }

  public void changed(SyncAction action) {
		int index = _data.indexOf(action);
		if(index >= 0) _view.invalidate(index);
	}
  
}
