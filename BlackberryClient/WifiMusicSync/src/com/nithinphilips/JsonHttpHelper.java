package com.nithinphilips;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

import javax.microedition.io.Connector;
import javax.microedition.io.HttpConnection;

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

}
