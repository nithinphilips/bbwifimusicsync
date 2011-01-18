package com.nithinphilips.wifimusicsync.components;

import net.rim.device.api.system.Characters;
import net.rim.device.api.ui.Field;
import net.rim.device.api.ui.FieldChangeListener;
import net.rim.device.api.ui.Font;
import net.rim.device.api.ui.component.ButtonField;
import net.rim.device.api.ui.component.CheckboxField;
import net.rim.device.api.ui.component.Dialog;
import net.rim.device.api.ui.component.RichTextField;
import net.rim.device.api.ui.component.SeparatorField;
import net.rim.device.api.ui.container.HorizontalFieldManager;
import net.rim.device.api.ui.container.PopupScreen;
import net.rim.device.api.ui.container.VerticalFieldManager;

import com.nithinphilips.wifimusicsync.model.PlaylistInfo;

public class PlaylistSelectionDialog extends PopupScreen {

	int result;
	CheckboxField[] checkBoxes;
	
	public PlaylistSelectionDialog(final PlaylistInfo[] choices) {
		super(new VerticalFieldManager(VERTICAL_SCROLL | VERTICAL_SCROLLBAR));
	
		RichTextField popupTitleField = null;

		popupTitleField = new RichTextField("Select Playlists to Sync", RichTextField.NON_FOCUSABLE);

		net.rim.device.api.ui.Font defaultFont = popupTitleField.getFont();
		popupTitleField.setFont(defaultFont.derive(Font.BOLD, defaultFont.getHeight() + 1));


		HorizontalFieldManager buttonFieldMan = new HorizontalFieldManager(HorizontalFieldManager.FIELD_RIGHT);

		ButtonField connectField = new ButtonField("OK", HorizontalFieldManager.FIELD_RIGHT);
		connectField.setChangeListener(new FieldChangeListener() {
			public void fieldChanged(Field field, int context) {
				
				for (int i = 0; i < checkBoxes.length; i++) {
					choices[i].setSelected(checkBoxes[i].getChecked());
				}
				
				result = Dialog.OK;
				close();
			}
		});

		ButtonField quitField = new ButtonField("Cancel", HorizontalFieldManager.FIELD_RIGHT);
		quitField.setChangeListener(new FieldChangeListener() {
			public void fieldChanged(Field field, int context) {
				result = Dialog.CANCEL;
				close();
			}
		});

		add(popupTitleField);
		add(new SeparatorField());
		checkBoxes = new CheckboxField[choices.length];
		for (int i = 0; i < choices.length; i++) {
			checkBoxes[i] = new CheckboxField(choices[i].toString(), choices[i].isSelected());
			add(checkBoxes[i]);
		}
		buttonFieldMan.add(connectField);
		buttonFieldMan.add(quitField);
		add(buttonFieldMan);
	}


	public boolean keyChar(char c, int status, int time) {
		if (c == Characters.ESCAPE) {
			result = Dialog.CANCEL;
			close();
			return true;	
		}
		return super.keyChar(c, status, time);
	}

	public int getResult() {
		return result;
	}
}
