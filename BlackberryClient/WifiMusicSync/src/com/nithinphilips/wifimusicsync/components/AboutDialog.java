package com.nithinphilips.wifimusicsync.components;

import net.rim.device.api.system.Characters;
import net.rim.device.api.ui.Field;
import net.rim.device.api.ui.FieldChangeListener;
import net.rim.device.api.ui.Font;
import net.rim.device.api.ui.component.ButtonField;
import net.rim.device.api.ui.component.CheckboxField;
import net.rim.device.api.ui.component.Dialog;
import net.rim.device.api.ui.component.LabelField;
import net.rim.device.api.ui.component.RichTextField;
import net.rim.device.api.ui.component.SeparatorField;
import net.rim.device.api.ui.container.HorizontalFieldManager;
import net.rim.device.api.ui.container.PopupScreen;
import net.rim.device.api.ui.container.VerticalFieldManager;

import com.nithinphilips.wifimusicsync.model.PlaylistInfo;

public class AboutDialog extends PopupScreen {

    public AboutDialog() {
        super(new VerticalFieldManager(VERTICAL_SCROLL | VERTICAL_SCROLLBAR));
    
        RichTextField popupTitleField = new RichTextField("About Music Sync", RichTextField.NON_FOCUSABLE);

        net.rim.device.api.ui.Font defaultFont = popupTitleField.getFont();
        popupTitleField.setFont(defaultFont.derive(Font.BOLD, defaultFont.getHeight() + 1));


        HorizontalFieldManager buttonFieldMan = new HorizontalFieldManager(HorizontalFieldManager.FIELD_RIGHT);

        ButtonField connectField = new ButtonField("OK", HorizontalFieldManager.FIELD_RIGHT);
        connectField.setChangeListener(new FieldChangeListener() {
            public void fieldChanged(Field field, int context) {
                close();
            }
        });

        add(popupTitleField);
        add(new SeparatorField());
        add(new LabelField("Revision $Rev$"));
        
        buttonFieldMan.add(connectField);
        add(buttonFieldMan);
    }


    public boolean keyChar(char c, int status, int time) {
        if (c == Characters.ESCAPE) {
            close();
            return true;    
        }
        return super.keyChar(c, status, time);
    }
}
