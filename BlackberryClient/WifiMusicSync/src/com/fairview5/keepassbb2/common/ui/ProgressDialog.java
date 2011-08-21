/*
 * KeePass for BlackBerry Copyright 2007 Fairview 5 Engineering, LLC <george.joseph@fairview5.com>
 * 
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either
 * version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 * PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

package com.fairview5.keepassbb2.common.ui;

import net.rim.device.api.system.Bitmap;
import net.rim.device.api.ui.UiApplication;
import net.rim.device.api.ui.component.BitmapField;
import net.rim.device.api.ui.component.LabelField;
import net.rim.device.api.ui.container.HorizontalFieldManager;
import net.rim.device.api.ui.container.PopupScreen;

public final class ProgressDialog extends PopupScreen
{
    public static ProgressDialog current;

    public LabelField            legend;
    public LabelField            pf;
    public Throwable             innerException;

    public ProgressDialog(final String title)
    {
        super(new HorizontalFieldManager(USE_ALL_WIDTH));
        legend = new LabelField(title, LabelField.FIELD_VCENTER);
        pf = new LabelField();

        BitmapField bmpf = new BitmapField(Bitmap.getPredefinedBitmap(Bitmap.HOURGLASS));

        add(bmpf);
        add(legend);
    }

    public static void showProgress(final String legend)
    {
        if (UiApplication.getUiApplication().isEventThread())
        {
            current = new ProgressDialog(legend);
            current.innerException = null;
            UiApplication.getUiApplication().pushScreen(current);
            return;
        }
        UiApplication.getUiApplication().invokeLater(new Runnable() {
            public void run()
            {
                current = new ProgressDialog(legend);
                current.innerException = null;
                UiApplication.getUiApplication().pushScreen(current);
            }
        });
    }

    public static void prepareModal(final String legend)
    {
        if (UiApplication.getUiApplication().isEventThread())
        {
            current = new ProgressDialog(legend);
            return;
        }
        UiApplication.getUiApplication().invokeLater(new Runnable() {
            public void run()
            {
                current = new ProgressDialog(legend);
            }
        });
    }

    public static void doModal()
    {
        if (UiApplication.getUiApplication().isEventThread())
        {
            UiApplication.getUiApplication().pushModalScreen(current);
            return;
        }
        UiApplication.getUiApplication().invokeLater(new Runnable() {
            public void run()
            {
                UiApplication.getUiApplication().pushModalScreen(current);
            }
        });
    }

    public static void setLegend(final String text)
    {
        if (current == null) return;
        if (UiApplication.getUiApplication().isEventThread())
        {
            current.legend.setText(text);
            return;
        }
        UiApplication.getUiApplication().invokeLater(new Runnable() {
            public void run()
            {
                current.legend.setText(text);
            }
        });
    }

    public static void setProgress(final String text)
    {
        if (current == null) return;
        if (UiApplication.getUiApplication().isEventThread())
        {
            current.pf.setText(text);
            return;
        }
        UiApplication.getUiApplication().invokeLater(new Runnable() {
            public void run()
            {
                current.pf.setText(text);
            }
        });
    }

    public static void closeProgress()
    {
        if (current == null) return;
        if (UiApplication.getUiApplication().isEventThread())
        {
            current.close();
            return;
        }
        UiApplication.getUiApplication().invokeLater(new Runnable() {
            public void run()
            {
                current.close();
            }
        });
    }

    public static void setInnerException(Throwable t)
    {
        if (current == null) return;
        current.innerException = t;
    }

    public static Throwable getInnerException()
    {
        if (current == null) return null;
        return current.innerException;
    }

    public static void conditionalThrow() throws Throwable
    {
        if (current == null) return;
        if (current.innerException == null) return;
        throw current.innerException;

    }

}