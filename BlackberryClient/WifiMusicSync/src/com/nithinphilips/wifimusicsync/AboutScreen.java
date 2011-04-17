package com.nithinphilips.wifimusicsync;

import net.rim.blackberry.api.browser.Browser;
import net.rim.device.api.system.Bitmap;
import net.rim.device.api.ui.Field;
import net.rim.device.api.ui.Graphics;
import net.rim.device.api.ui.Color;
import net.rim.device.api.ui.Font;
import net.rim.device.api.ui.MenuItem;
import net.rim.device.api.ui.component.BitmapField;
import net.rim.device.api.ui.component.LabelField;
import net.rim.device.api.ui.component.RichTextField;
import net.rim.device.api.ui.component.SeparatorField;
import net.rim.device.api.ui.container.HorizontalFieldManager;
import net.rim.device.api.ui.container.MainScreen;
import net.rim.device.api.ui.container.VerticalFieldManager;
import net.rim.device.api.ui.decor.BackgroundFactory;
import net.rim.device.api.util.StringProvider;

public class AboutScreen extends MainScreen
{

    public AboutScreen()
    {

        net.rim.device.api.ui.Font defaultFont = this.getFont();
        net.rim.device.api.ui.Font titleFont = defaultFont.derive(Font.BOLD, defaultFont.getHeight() + 5);
        net.rim.device.api.ui.Font tinyFont = defaultFont.derive(0, defaultFont.getHeight() - 5);
        net.rim.device.api.ui.Font medFont = defaultFont.derive(0, defaultFont.getHeight() - 3);
        
        
        
        MenuItem readGplItem = new MenuItem(new StringProvider("Read GPL License"), 1000000, 1){
            public void run()
            {
                Browser.getDefaultSession().displayPage("http://www.gnu.org/licenses/gpl.html");
            }
        };
        
        MenuItem visitUltimateGnomeItem = new MenuItem(new StringProvider("Visit Ultimate-Gnome Icons Website"), 100000, 100){
            public void run()
            {
                Browser.getDefaultSession().displayPage("https://code.google.com/p/ultimate-gnome/");
            }
        };
        
        MenuItem visitFaenzaItem = new MenuItem(new StringProvider("Visit Faenza Icons Website"), 100000, 100){
            public void run()
            {
                Browser.getDefaultSession().displayPage("http://tiheum.deviantart.com/art/Faenza-Icons-173323228");
            }
        };
        
        MenuItem visitElementaryItem = new MenuItem(new StringProvider("Visit Elementary Icons Website"), 100000, 100){
            public void run()
            {
                Browser.getDefaultSession().displayPage("http://danrabbit.deviantart.com/art/elementary-Icons-65437279");
            }
        };
        
        
        addMenuItem(readGplItem);
        addMenuItem(visitUltimateGnomeItem);
        addMenuItem(visitFaenzaItem);
        addMenuItem(visitElementaryItem);
        
        
        ((VerticalFieldManager) getMainManager()).setBackground(BackgroundFactory.createSolidBackground(Color.BLACK));
        VerticalFieldManager container = new VerticalFieldManager() {
            public void paint(Graphics graphics)
            {
                graphics.setColor(Color.WHITE);
                super.paint(graphics);
            }
        };
        container.setMargin(5, 5, 5, 5);

        BitmapField iconField = new BitmapField(Bitmap.getBitmapResource("music-sync-68.png"));
        RichTextField popupTitleField = new RichTextField("Music Sync", RichTextField.NON_FOCUSABLE | Field.FIELD_BOTTOM);
        
        HorizontalFieldManager headerContainer = new HorizontalFieldManager();
        headerContainer.add(iconField);
        headerContainer.add(popupTitleField);

        popupTitleField.setFont(titleFont);
        popupTitleField.setMargin(0, 0, 5, 0);

        RichTextField technicalTitleField = new RichTextField("Release Details", RichTextField.NON_FOCUSABLE);
        technicalTitleField.setFont(defaultFont.derive(Font.BOLD, defaultFont.getHeight() + 1));
        technicalTitleField.setMargin(5, 0, 0, 0);

        LabelField copyrightField = new LabelField("(C) 2011 Nithin Philips.");
        LabelField descriptionField = new LabelField("Music Sync allows you to wirelessly sync your iTunes playlists and with your Blackberry device.");
        
        LabelField iconsInfoField = new LabelField("Icons shamelessy ripped from Ultimate-Gnome, Faenza and Elementary icon themes. This application uses components from KeePassBB <http://f5bbutils.fairview5.com/keepassbb2/>");
        LabelField gnuField0 = new LabelField(
                "This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.");
        LabelField gnuField1 = new LabelField(
                "This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.");
        LabelField gnuField2 = new LabelField(
                "You should have received a copy of the GNU General Public License along with this program. If not, see <http://www.gnu.org/licenses/>.");
        
        LabelField dateField = new LabelField("Build Date: Sun, 30 Jan 2011");
        LabelField revField = new LabelField("Build Revision: $Rev$");
        
        copyrightField.setFont(medFont);
        descriptionField.setFont(tinyFont);
        iconsInfoField.setFont(tinyFont);
        gnuField0.setFont(tinyFont);
        gnuField1.setFont(tinyFont);
        gnuField2.setFont(tinyFont);
        dateField.setFont(tinyFont);
        revField.setFont(tinyFont);
        
        descriptionField.setMargin(5, 0, 0, 0);
        iconsInfoField.setMargin(5, 0, 0, 0);
        gnuField0.setMargin(5, 0, 0, 0);

        container.add(headerContainer);
        container.add(new SeparatorField());
        container.add(copyrightField);
        container.add(descriptionField);
        
        container.add(iconsInfoField);
        container.add(gnuField0);
        container.add(gnuField1);
        container.add(gnuField2);
        container.add(technicalTitleField);
        container.add(new SeparatorField());
        container.add(dateField);
        container.add(revField);

        add(container);
    }
}
