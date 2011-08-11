package com.nithinphilips;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

import javax.microedition.io.Connector;
import javax.microedition.io.HttpConnection;

import net.rim.device.api.servicebook.ServiceBook;
import net.rim.device.api.servicebook.ServiceRecord;
import net.rim.device.api.system.CoverageInfo;
import net.rim.device.api.system.DeviceInfo;
import net.rim.device.api.system.WLANInfo;

public class JsonHttpHelper {

	
    public static String URL_SUFFIX;
    protected static final boolean USE_MDS_IN_SIMULATOR = true;
    
    static {
        if(Debug.DEBUG){
            URL_SUFFIX = ";deviceside=false;ConnectionTimeout=10000";
        }else{
            URL_SUFFIX = ";deviceside=true;interface=wifi;ConnectionTimeout=10000";
        }
    }
	
	public static String getVfsAccessUrl(String url){
		return url.substring(0, url.lastIndexOf('/')) + "/vfs/";
	}
	
	public static String convertVfsToHttp(String vfsAccessUrl, String vfsUrl){
		return vfsAccessUrl + vfsUrl;
	}
	
	/**
	 * Executes a command.
	 * @param url The full url of the command, including any parameters
	 * @return
	 */
	public static String executeCommand(String url, String jsonData) {
		byte[] serverResponse = httpPost(url, jsonData);

		if (serverResponse == null) {
			return "";
		} else {
			return new String(serverResponse);
		}
	}
	
	protected static byte[] httpPost(String url, String content) {
		HttpConnection connection = null;
		byte[] result = null;

		try {
			connection = (HttpConnection) Connector.open(url + URL_SUFFIX);
			// HTTP Request
			connection.setRequestMethod(HttpConnection.POST);
			connection.setRequestProperty("Content-Type","application/json"); 
			connection.setRequestProperty("Connection", "close");
			connection.setRequestProperty("Content-Length", String.valueOf(content.length()));

			// Post content
			OutputStream oStrm = connection.openOutputStream();
			oStrm.write(content.getBytes());
			oStrm.flush(); 
			
			//return readRespose(connection);
			final ByteBuffer bb = new ByteBuffer(connection.openInputStream());

		    // run this in EDT... not BGT!
		    return bb.getBytes();

			
		} catch (IOException error) {
			System.out.println(error.toString());
			error.printStackTrace();
		} finally {
			if (connection != null) {
				try {
					connection.close();
				} catch (Exception error) {
					/* log error */
				}
			}
		}
		return result;
	}
	
	public static byte[] httpGet(String url) {
		HttpConnection connection = null;
		byte[] result = null;

		try {
			connection = (HttpConnection) Connector.open(url + URL_SUFFIX);
			// HTTP Request
			connection.setRequestMethod(HttpConnection.GET);
			connection.setRequestProperty("Connection", "close");

			//return readRespose(connection);
			final ByteBuffer bb = new ByteBuffer(connection.openInputStream());

		    // run this in EDT... not BGT!
		    return bb.getBytes();

			
		} catch (IOException error) {
			System.out.println(error.toString());
			error.printStackTrace();
		} finally {
			if (connection != null) {
				try {
					connection.close();
				} catch (Exception error) {
					/* log error */
				}
			}
		}
		return result;
	}
	
	protected static byte[] readRespose(HttpConnection connection) throws IOException {
		InputStream inputstream = null;
		byte[] result = null;
		try{
			// HTTP Response
			if (connection.getResponseCode() == HttpConnection.HTTP_OK) {
				inputstream = connection.openInputStream();
				int length = (int) connection.getLength();
				if (length != -1) {
					byte incomingData[] = new byte[length];
					inputstream.read(incomingData);
					result = incomingData;
				} else {
					ByteArrayOutputStream bytestream = new ByteArrayOutputStream();
					int ch;
					while ((ch = inputstream.read()) != -1) {
						bytestream.write(ch);
					}
					result = bytestream.toByteArray();
					bytestream.close();
				}
			}
		} finally {
			if (inputstream != null) {
				try {
					inputstream.close();
				} catch (Exception error) {
					/* log error */
				}
			}
		}
		return result;
	}
	
	
	/**
     * Determines what connection type to use and returns the necessary string to use it.
     * @return A string with the connection info
     */
    private static String getConnectionString()
    {
        // This code is based on the connection code developed by Mike Nelson of AccelGolf.
        // http://blog.accelgolf.com/2009/05/22/blackberry-cross-carrier-and-cross-network-http-connection        
        String connectionString = null;                
                        
        // Simulator behavior is controlled by the USE_MDS_IN_SIMULATOR variable.
        if(DeviceInfo.isSimulator())
        {
                if(USE_MDS_IN_SIMULATOR)
                {
                        logMessage("Device is a simulator and USE_MDS_IN_SIMULATOR is true");
                        connectionString = ";deviceside=false";                 
                }
                else
                {
                        logMessage("Device is a simulator and USE_MDS_IN_SIMULATOR is false");
                        connectionString = ";deviceside=true";
                }
        }                                        
                
        // Wifi is the preferred transmission method
        else if(WLANInfo.getWLANState() == WLANInfo.WLAN_STATE_CONNECTED)
        {
            logMessage("Device is connected via Wifi.");
            connectionString = ";interface=wifi";
        }
                        
        // Is the carrier network the only way to connect?
        else if((CoverageInfo.getCoverageStatus() & CoverageInfo.COVERAGE_DIRECT) == CoverageInfo.COVERAGE_DIRECT)
        {
            logMessage("Carrier coverage.");
                        
            String carrierUid = getCarrierBIBSUid();
            if(carrierUid == null) 
            {
                // Has carrier coverage, but not BIBS.  So use the carrier's TCP network
                logMessage("No Uid");
                connectionString = ";deviceside=true";
            }
            else 
            {
                // otherwise, use the Uid to construct a valid carrier BIBS request
                logMessage("uid is: " + carrierUid);
                connectionString = ";deviceside=false;connectionUID="+carrierUid + ";ConnectionType=mds-public";
            }
        }                
        
        // Check for an MDS connection instead (BlackBerry Enterprise Server)
        else if((CoverageInfo.getCoverageStatus() & CoverageInfo.COVERAGE_MDS) == CoverageInfo.COVERAGE_MDS)
        {
            logMessage("MDS coverage found");
            connectionString = ";deviceside=false";
        }
        
        // If there is no connection available abort to avoid bugging the user unnecssarily.
        else if(CoverageInfo.getCoverageStatus() == CoverageInfo.COVERAGE_NONE)
        {
            logMessage("There is no available connection.");
        }
        
        // In theory, all bases are covered so this shouldn't be reachable.
        else
        {
            logMessage("no other options found, assuming device.");
            connectionString = ";deviceside=true";
        }        
        
        return connectionString;
    }
    
	/**
     * Looks through the phone's service book for a carrier provided BIBS network
     * @return The uid used to connect to that network.
     */
    private static String getCarrierBIBSUid()
    {
        ServiceRecord[] records = ServiceBook.getSB().getRecords();
        int currentRecord;
        
        for(currentRecord = 0; currentRecord < records.length; currentRecord++)
        {
            if(records[currentRecord].getCid().toLowerCase().equals("ippp"))
            {
                if(records[currentRecord].getName().toLowerCase().indexOf("bibs") >= 0)
                {
                    return records[currentRecord].getUid();
                }
            }
        }
        
        return null;
    }    

    private static void logMessage(String string) {
		// TODO Auto-generated method stub
	}
    
}
