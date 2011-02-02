package com.nithinphilips.wifimusicsync.components;

import net.rim.device.api.ui.Color;
import net.rim.device.api.ui.Font;
import net.rim.device.api.ui.Graphics;
import net.rim.device.api.ui.component.CheckboxField;
import net.rim.device.api.ui.component.Dialog;
import net.rim.device.api.ui.component.RichTextField;
import net.rim.device.api.ui.component.SeparatorField;
import net.rim.device.api.ui.container.MainScreen;
import net.rim.device.api.ui.container.VerticalFieldManager;
import net.rim.device.api.ui.decor.BackgroundFactory;

import com.nithinphilips.wifimusicsync.model.PlaylistInfo;

public class PlaylistSelectionDialog extends MainScreen
{

    int             result;
    CheckboxField[] checkBoxes;
    PlaylistInfo[]  choices;

    public PlaylistSelectionDialog(PlaylistInfo[] choices, String title)
    {
        this.choices = choices;

        ((VerticalFieldManager) getMainManager()).setBackground(BackgroundFactory.createSolidBackground(Color.BLACK));
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

        container.add(popupTitleField);
        container.add(new SeparatorField());
        checkBoxes = new CheckboxField[choices.length];
        for (int i = 0; i < choices.length; i++)
        {
            checkBoxes[i] = new CheckboxField(choices[i].toString(), choices[i].isSelected());
            container.add(checkBoxes[i]);
        }
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
