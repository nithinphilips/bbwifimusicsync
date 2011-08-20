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

import java.util.Vector;

/**
 * Utility class to work with strings. 
 */
public final class StringUtils {

    private StringUtils() {
    }

    /**
     * Checks if the given string is empty.
     * String is considered as empty if it consists of whitespace characters only also. 
     * @param s string to check
     * @return <code>true</code> if string is empty, otherwise <code>false</code>
     */
    public static boolean isEmpty(String s) {
        return (s == null || s.trim().length() == 0);
    }
    
    /**
     * Checks if two strings are equal by their values.
     * One of strings or both can be <code>null</code>.
     * If both strings are <code>null</code>, they are considered as equal.
     * @param s1 one string
     * @param s2 another string
     * @return <code>true</code> if strings are equal, otherwise <code>false</code>
     */
    public static boolean isEqual(String s1, String s2) {
        if (s1 == null && s2 == null) {
            return true;
        }
        if (s1 == null || s2 == null) {
            return false;
        }
        return s1.equals(s2);
    }
    
    /**
     * Splits a string to parts separated by the given character. 
     * @param str string to split
     * @param separator character that is considered as separator
     * @return array of substrings of the given string 
     */
    public static String[] split(String str, char separator) {
        int k;
        int l = 0;
        Vector tokens = new Vector();
        
        while ((k = str.indexOf(separator, l)) != -1) {
            if (k > l) {
                tokens.addElement(str.substring(l, k));
            }
            l = k + 1;
            if (l == str.length()) {
                break;
            }
        }
        
        if (l < str.length()) {
            tokens.addElement(str.substring(l));
        }
        
        String[] tokenArray = new String[tokens.size()];
        tokens.copyInto(tokenArray);
        return tokenArray;
    }
    
    /**
     * Splits a string to parts separated by the given separators.
     * @param str string to split
     * @param separators a string that contains characters used as separators
     * @return array of substrings of the given string
     */
    public static String[] split(String str, String separators) {
        int k;
        int l = 0;
        Vector tokens = new Vector();
        
        while ((k = minimalIndexOf(str, separators, l)) != -1) {
            if (k > l) {
                tokens.addElement(str.substring(l, k));
            }
            l = k + 1;
            if (l == str.length()) {
                break;
            }
        }
        
        if (l < str.length()) {
            tokens.addElement(str.substring(l));
        }
        
        String[] tokenArray = new String[tokens.size()];
        tokens.copyInto(tokenArray);
        return tokenArray;
    }
    
    private static int minimalIndexOf(String str, String separators, int startIndex) {
        int k = -1;
        for (int i = 0; i < separators.length(); i++) {
            int t = str.indexOf(separators.charAt(i), startIndex);
            if (t != -1) {
                k = (k == -1) ? t : Math.min(k, t);
            }
        }
        return k;
    }
    
    /**
     * Replaces within the given string one fragment with another one.
     * @param str string under operation
     * @param toReplace string fragment to be replaced
     * @param replacement string fragment that will replace
     * @return new string with replaced fragment, 
     * or string with previous value if there is nothing to replace 
     */
    public static String replace(String str, String toReplace, String replacement) {
        if (str == null || toReplace == null || replacement == null) {
            throw new IllegalArgumentException("Can't replace: one of argument is null");
        }
        
        StringBuffer buffer = new StringBuffer();
        int lastIndex = 0;
        int k;

        while (lastIndex < str.length() 
                && (k = str.indexOf(toReplace, lastIndex)) != -1) {
            buffer.append(str.substring(lastIndex, k)).append(replacement);
            lastIndex = k + toReplace.length();
        }
        
        if (lastIndex < str.length()) {
            buffer.append(str.substring(lastIndex, str.length()));
        }
        
        return buffer.toString();
    }
    
    /**
     * Checks if the given string contains whitespace characters.
     * @param s string to check
     * @return <code>true</code> if the string contains whitespaces, otherwise <code>false</code>
     */
    public static boolean containsWhitespaces(String s) {
        final char[] whitespaces = new char[] {' ', '\t', '\n', '\r', '\f'};
        for (int i = 0; i < whitespaces.length; i++) {
            if (s.indexOf(whitespaces[i]) != -1) {
                return true;
            }
        }
        return false;
    }
    
    public static String formatPhoneNumber(String number) {
    	
    	if(isEmpty(number)) return "";
    	
    	String phoneNumber = number.trim();
    	if(phoneNumber.length() == 11){
    		StringBuffer buf = new StringBuffer();
    		buf.append(phoneNumber.substring(0, 1));
    		buf.append("(");
    		buf.append(phoneNumber.substring(1, 4));
    		buf.append(")");
    		buf.append(phoneNumber.substring(4, 7));
    		buf.append("-");
    		buf.append(phoneNumber.substring(7));
    		return buf.toString();
    	}
    	
    	if(phoneNumber.length() == 10){
    		StringBuffer buf = new StringBuffer();
    		buf.append("(");
    		buf.append(phoneNumber.substring(0, 3));
    		buf.append(")");
    		buf.append(phoneNumber.substring(3, 6));
    		buf.append("-");
    		buf.append(phoneNumber.substring(6));
    		return buf.toString();
    	}
    	
    	if(phoneNumber.length() == 7){
    		StringBuffer buf = new StringBuffer();
    		buf.append(phoneNumber.substring(0, 3));
    		buf.append("-");
    		buf.append(phoneNumber.substring(3));
    		return buf.toString();
    	}
    	
    	return phoneNumber;
    }
}
