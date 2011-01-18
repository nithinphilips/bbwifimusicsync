package com.nithinphilips;

import net.rim.device.api.ui.Color;
import net.rim.device.api.ui.Field;
import net.rim.device.api.ui.Font;
import net.rim.device.api.ui.Graphics;
import net.rim.device.api.ui.Manager;
import net.rim.device.api.ui.XYEdges;
import net.rim.device.api.ui.component.EditField;
import net.rim.device.api.ui.component.LabelField;
import net.rim.device.api.ui.container.HorizontalFieldManager;
import net.rim.device.api.ui.container.VerticalFieldManager;
import net.rim.device.api.ui.decor.BackgroundFactory;
import net.rim.device.api.ui.decor.Border;
import net.rim.device.api.ui.decor.BorderFactory;

/**
 * A helper class to create a nice looking preference or data edit screen.
 * @author Nithin
 */
public final class UiFactory {
	
	public static final int COLOR_SCREEN_BACKGROUND = 0x00dedfde;
	
	public static final int COLOR_GROUP_LABEL_FOREGROUND = 0x007b7d84;
	public static final int COLOR_GROUP_LABEL_BORDER = 0x00c6c3c6;
	public static final int COLOR_EDITFIELD_BORDER = 0x00d6dbde;
	public static final int COLOR_GROUP_BEZEL_OUTER = 0x00d6d6d6;
	public static final int COLOR_GROUP_BEZEL_INNER = 0x00c6c3c6;
	
	
	/**
	 * Arranges a label and a field horizontally using a HorizontalFieldManager. Label first,then the field.
	 * @param label The text of the label.
	 * @param field The field to place.
	 * @return A HorizontalFieldManager containing both label and the field.
	 */
	public static HorizontalFieldManager arrangeHorizontally(String label, Field field){
    	HorizontalFieldManager manager = new HorizontalFieldManager();
    	
    	manager.add(new LabelField(label, LabelField.USE_ALL_HEIGHT | LabelField.FIELD_VCENTER | LabelField.NON_FOCUSABLE));
    	manager.add(field);
    	
        return manager;
    }
    
	/**
	 * Creates an edit field with no label and a rounded border and input restricted to numbers and decimals.
	 * @param text The default text to show in the edit field.
	 * @return A new field.
	 */
	public static EditField createNumericEditField(String text){
    	return createEditField(text, EditField.FILTER_REAL_NUMERIC);
    }
    
	/**
	 * Creates an edit field with no label and a rounded border.
	 * @param text The default text to show in the edit field.
	 * @param style The style bitmask to apply to the field.
	 * @return A new field.
	 */
	public static EditField createEditField(String text, long style){
    	EditField field = new EditField ("", text, EditField.DEFAULT_MAXCHARS, style);
    	field.setBorder(BorderFactory.createRoundedBorder(new XYEdges(5, 5, 5, 5), COLOR_EDITFIELD_BORDER, Border.STYLE_SOLID));
    	field.setMargin(new XYEdges(2, 0, 3, 0));
    	return field;
    }
    
	/**
     * Create a VerticalFieldManager and applies a visual style to make it appear like a distinct group. 
     * @param title The title of the group. If null or empty, no label will be added. 
     * @return The new VerticalFieldManager.
     */
    public static VerticalFieldManager createVerticalFieldGroup(String title){
    	return (VerticalFieldManager)makeGroup(new VerticalFieldManager(), title);
    }
    
    /**
     * Applies a visual style to a Manager to make it appear like a distinct group. 
     * @param manager The Manager to transform.
     * @param title The title of the group. If null or empty, no label will be added. 
     * @return Reference to the same Manager object that was passed as the manager parameter.
     */
    public static Manager makeGroup(Manager manager, String title){
    	if((title != null) && (title != "")){
	        manager.add(createGroupLabel(title));
    	}
        
        manager.setMargin(new XYEdges(3, 3, 0, 3));
        manager.setPadding(new XYEdges(5, 5, 5, 5));
        manager.setBackground(BackgroundFactory.createSolidBackground(Color.WHITE));
        manager.setBorder(BorderFactory.createBevelBorder(
        		new XYEdges(2, 2, 2, 2), 
        		new XYEdges(COLOR_GROUP_BEZEL_OUTER, COLOR_GROUP_BEZEL_OUTER, COLOR_GROUP_BEZEL_OUTER, COLOR_GROUP_BEZEL_OUTER),
        		new XYEdges(COLOR_GROUP_BEZEL_INNER, COLOR_GROUP_BEZEL_INNER, COLOR_GROUP_BEZEL_INNER, COLOR_GROUP_BEZEL_INNER)));
        
        return manager;
    }
    
    /**
     * Create a LabelField that spans the entire width of the container with gray text and a single pixel gray border at the bottom. 
     * @param title
     * @return
     */
    public static LabelField createGroupLabel(String title){
    	LabelField titleField = new LabelField(title, LabelField.USE_ALL_WIDTH){
        	protected void paint(Graphics graphics) {
        		graphics.setColor(COLOR_GROUP_LABEL_FOREGROUND);
        		super.paint(graphics);
        	}
        };
        Font font = titleField.getFont();
        titleField.setFont(font.derive(Font.BOLD, font.getHeight() - 3));
        titleField.setPadding(new XYEdges(0, 0, 4, 0));
        titleField.setBorder(BorderFactory.createSimpleBorder(new XYEdges(0, 0, 1, 0), new XYEdges(0, 0, COLOR_GROUP_LABEL_BORDER, 0), Border.STYLE_SOLID));
        titleField.setMargin(new XYEdges(0, 0, 4, 0));
        return titleField;
    
    }
}
