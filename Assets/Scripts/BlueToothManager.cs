using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Robot.Singleton;

public class BlueToothManager : Singleton<BlueToothManager> {

    public List<DeviceObject> DeviceAddressList = new List<DeviceObject> ();

    private string _connectedID = "74:DA:EA:B3:E6:5C";
    private string _readCharacteristicUUID = "ffe1";
    private string _writeCharacteristicUUID = "ffe1";

    private string _address = "";
    private string _serviceUUID = "";
    private string _characteristicUUID = "";
    private float _subscribingTimeout = 0f;

    public void Scan (GameObject TagetScrollContent, GameObject prefab, System.Action SelectAfterEvent ) {

        BluetoothLEHardwareInterface.Initialize ( true, false, () => {

            //DeviceAddressList = new List<DeviceObject> ();

            BluetoothLEHardwareInterface.ScanForPeripheralsWithServices ( null, ( address, name ) => {

                DeviceAddressList.Add ( new DeviceObject ( address, name ) );

            }, null );

        }, ( error ) => {

            BluetoothLEHardwareInterface.Log ( "BLE Error: " + error );
            Debug.Log ( "BLE Error: " + error );
        } );

        StartCoroutine ( scanDelay ( TagetScrollContent , prefab , SelectAfterEvent ) );
    }

    IEnumerator scanDelay ( GameObject TagetScrollContent , GameObject prefab, System.Action SelectAfterEvent) {

        yield return new WaitForSeconds ( 3f );

        GameObjectAgent.Instance.getComponent<Canvas>(GameObject.Find("BlueToothGUI")).enabled = true;
        
        Debug.Log ( DeviceAddressList.Count );

        foreach ( var list in DeviceAddressList ) {

            GameObject go = GUIAgent.Instance.SetActionButton ( prefab, list.Name, list.Name, null, TagetScrollContent );
            go.GetComponent<UnityEngine.UI.Button> ().onClick.AddListener ( () => {
                Connect ( list.Address );
                Debug.Log ( "connect" );
                SelectAfterEvent ();
            } );
        }

    }
    public void Connect ( string addressID ) {

        BluetoothLEHardwareInterface.ConnectToPeripheral ( addressID, ( address ) => {
        },
                ( address, serviceUUID ) => {
                },
                ( address, serviceUUID, characteristicUUID ) => {

                    //_readCharacteristicUUID = characteristicUUID;
                    //_writeCharacteristicUUID = characteristicUUID;

                    _address = address;
                    _serviceUUID = serviceUUID;
                    _characteristicUUID = characteristicUUID;

                    BluetoothLEHardwareInterface.Log ( "address :" + address );
                    BluetoothLEHardwareInterface.Log ( "serviceUUID :" + serviceUUID );
                    BluetoothLEHardwareInterface.Log ( "characteristicUUID :" + characteristicUUID );
                    // discovered characteristic

                }, ( address ) => {

                    // this will get called when the device disconnects
                    // be aware that this will also get called when the disconnect
                    // is called above. both methods get call for the same action
                    // this is for backwards compatibility

                } );

    }


    public void Connect () {

        BluetoothLEHardwareInterface.ConnectToPeripheral ( _connectedID, ( address ) => {
        },
                ( address, serviceUUID ) => {
                },
                ( address, serviceUUID, characteristicUUID ) => {

                    //_readCharacteristicUUID = characteristicUUID;
                    //_writeCharacteristicUUID = characteristicUUID;

                    _address = address;
                    _serviceUUID = serviceUUID;
                    _characteristicUUID = characteristicUUID;

                    BluetoothLEHardwareInterface.Log ( "address :" + address );
                    BluetoothLEHardwareInterface.Log ( "serviceUUID :" + serviceUUID );
                    BluetoothLEHardwareInterface.Log ( "characteristicUUID :" + characteristicUUID );
                    // discovered characteristic

                }, ( address ) => {

                    // this will get called when the device disconnects
                    // be aware that this will also get called when the disconnect
                    // is called above. both methods get call for the same action
                    // this is for backwards compatibility

                } );
    }

    void Update () {
        // if ( _readFound && _writeFound ) {
        // _readFound = false;
        // _writeFound = false;

        // _subscribingTimeout = 1f;
        // }

        if ( _subscribingTimeout > 0f ) {
            _subscribingTimeout -= Time.deltaTime;
            if ( _subscribingTimeout <= 0f ) {
                _subscribingTimeout = 0f;

                BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress ( _connectedID, FullUUID ( _serviceUUID ), FullUUID ( _readCharacteristicUUID ), ( deviceAddress, notification ) => {

                }, ( deviceAddress2, characteristic, data ) => {

                    BluetoothLEHardwareInterface.Log ( "id: " + _connectedID );
                    if ( deviceAddress2.CompareTo ( _connectedID ) == 0 ) {
                        BluetoothLEHardwareInterface.Log ( string.Format ( "data length: {0}", data.Length ) );
                        if ( data.Length == 0 ) {
                        } else {
                            string s = System.Text.ASCIIEncoding.UTF8.GetString ( data );
                            BluetoothLEHardwareInterface.Log ( "data: " + s );
                            //Input2.text += s;
                        }
                    }

                } );

                //SendButton.SetActive ( true );
            }
        }
    }

    string FullUUID ( string uuid ) {
        return "0000" + uuid + "-0000-1000-8000-00805f9b34fb";
    }

    bool IsEqual ( string uuid1, string uuid2 ) {
        if ( uuid1.Length == 4 )
            uuid1 = FullUUID ( uuid1 );
        if ( uuid2.Length == 4 )
            uuid2 = FullUUID ( uuid2 );

        return ( uuid1.ToUpper ().CompareTo ( uuid2.ToUpper () ) == 0 );
    }

    void SendByte ( byte value ) {
        byte[] data = new byte[] { value };
        BluetoothLEHardwareInterface.WriteCharacteristic ( _connectedID, FullUUID ( _serviceUUID ), FullUUID ( _writeCharacteristicUUID ), data, data.Length, true, ( characteristicUUID ) => {

            BluetoothLEHardwareInterface.Log ( "Write Succeeded" );
        } );
    }

    void SendBytes ( byte[] data ) {
        BluetoothLEHardwareInterface.Log ( string.Format ( "data length: {0} uuid: {1}", data.Length.ToString (), FullUUID ( _writeCharacteristicUUID ) ) );
        BluetoothLEHardwareInterface.WriteCharacteristic ( _connectedID, FullUUID ( _serviceUUID ), FullUUID ( _writeCharacteristicUUID ), data, data.Length, true, ( characteristicUUID ) => {

            BluetoothLEHardwareInterface.Log ( "Write Succeeded" );
        } );
    }

    public void Send ( string msg ) {

        byte[] data = System.Text.Encoding.UTF8.GetBytes ( msg );

        BluetoothLEHardwareInterface.Log ( string.Format ( "data length: {0} uuid: {1}", data.Length.ToString (), _characteristicUUID ) );
        BluetoothLEHardwareInterface.WriteCharacteristic ( _address, _serviceUUID, _characteristicUUID, data, data.Length, true, ( characteristicUUID ) => {

            BluetoothLEHardwareInterface.Log ( "Write Succeeded" );
        } );

    }
}



/*

// Update is called once per frame
void Update () {
    // if ( _readFound && _writeFound ) {
    // _readFound = false;
    // _writeFound = false;

    // _subscribingTimeout = 1f;
    // }

    if ( _subscribingTimeout > 0f ) {
        _subscribingTimeout -= Time.deltaTime;
        if ( _subscribingTimeout <= 0f ) {
            _subscribingTimeout = 0f;

            BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress ( _connectedID, FullUUID ( _serviceUUID ), FullUUID ( _readCharacteristicUUID ), ( deviceAddress, notification ) => {

            }, ( deviceAddress2, characteristic, data ) => {

                BluetoothLEHardwareInterface.Log ( "id: " + _connectedID );
                if ( deviceAddress2.CompareTo ( _connectedID ) == 0 ) {
                    BluetoothLEHardwareInterface.Log ( string.Format ( "data length: {0}", data.Length ) );
                    if ( data.Length == 0 ) {
                    } else {
                        string s = System.Text.ASCIIEncoding.UTF8.GetString ( data );
                        BluetoothLEHardwareInterface.Log ( "data: " + s );
                        //Input2.text += s;
                    }
                }

            } );

            //SendButton.SetActive ( true );
        }
    }

}
*/
