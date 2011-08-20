/*
Copyright 2007-2010 WebDriver committers
Copyright 2007-2010 Google Inc.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

package org.openqa.selenium.remote.me.util;

import net.rim.device.api.system.EventLogger;

/**
 * Simple logger.
 * In debug mode logs to the system out.
 * When WebDriver is running in the BlackBerry device logs to the event log.
 */
public final class Logger {

    public static final long GUID = 0xe62bb1569737d974L; // = com.nithinphilips.wifimusicsync.Logger
    
    public static final String APP_NAME = "WiFi Music Sync";
    
    /**
     * Registers the WebDriver application in the BlackBerry event log.
     */
    public static void register() {
        EventLogger.register(GUID, APP_NAME, EventLogger.VIEWER_STRING);
    }
    
    /**
     * Logs debug message.
     * @param message string value
     */
    public static void info(String message) {
        System.out.println(APP_NAME + " #I# " + message);
        EventLogger.logEvent(GUID, message.getBytes(), EventLogger.INFORMATION);
    }
    
    /**
     * Logs debug message.
     * @param message string value
     */
    public static void debug(String message) {
        System.out.println(APP_NAME + " #D# " + message);
        EventLogger.logEvent(GUID, message.getBytes(), EventLogger.DEBUG_INFO);
    }
    
    /**
     * Logs error message.
     * @param message string value
     */
    public static void error(String message) {
        System.out.println(APP_NAME + " #E# " + message);
        EventLogger.logEvent(GUID, message.getBytes(), EventLogger.ERROR);
    }
}
