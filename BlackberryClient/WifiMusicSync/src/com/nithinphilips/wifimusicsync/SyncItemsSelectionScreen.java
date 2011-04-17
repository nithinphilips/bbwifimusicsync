package com.nithinphilips.wifimusicsync;

import net.rim.device.api.ui.Color;
import net.rim.device.api.ui.Font;
import net.rim.device.api.ui.Graphics;
import net.rim.device.api.ui.component.CheckboxField;
import net.rim.device.api.ui.component.RichTextField;
import net.rim.device.api.ui.component.SeparatorField;
import net.rim.device.api.ui.container.MainScreen;
import net.rim.device.api.ui.container.VerticalFieldManager;
import net.rim.device.api.ui.decor.BackgroundFactory;

import com.nithinphilips.wifimusicsync.model.PlaylistInfo;

public class SyncItemsSelectionScreen extends MainScreen
{

    int             result;
    CheckboxField[] checkBoxes;
    PlaylistInfo[]  choices;

    public SyncItemsSelectionScreen(PlaylistInfo[] choices, String title)
    {
        this.choices = choices;

        ((VerticalFieldManager) getMainManager()).setBackground(BackgroundFactory.createSolidBackground(Color.BLACK));
        
        VerticalFieldManager checkItemsContainer = new VerticalFieldManager() {
            public void paint(Graphics graphics)
            {
                graphics.setColor(Color.WHITE);
                super.paint(graphics);
            }
        };
        
        VerticalFieldManager container = new VerticalFieldManager() {
            public void paint(Graphics graphics)
            {
                graphics.setColor(Color.WHITE);
                super.paint(graphics);
            }
        };

        RichTextField popupTitleField = new RichTextField(title, RichTextField.NON_FOCUSABLE);
        popupTitleField.setMargin(5, 5, 5, 5);

        net.rim.device.api.ui.Font defaultFont = popupTitleField.getFont();
        popupTitleField.setFont(defaultFont.derive(Font.BOLD, defaultFont.getHeight() + 1));

        
        container.add(new SeparatorField());
        checkBoxes = new CheckboxField[choices.length];
        for (int i = 0; i < choices.length; i++)
        {
            checkBoxes[i] = new CheckboxField(choices[i].toString(), choices[i].isSelected());
            if(choices[i].isSelected()){
                checkItemsContainer.add(checkBoxes[i]);
            }else{
                container.add(checkBoxes[i]);
            }
        }
        
        add(popupTitleField);
        add(checkItemsContainer);
        add(new SeparatorField());
        add(container);
    }

    public int getResult()
    {
        return result;
    }

    public void save()
    {
        for (int i = 0; i < checkBoxes.length; i++)
        {
            choices[i].setSelected(checkBoxes[i].getChecked());
        }
    }
}
