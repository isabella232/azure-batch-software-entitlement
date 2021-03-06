# Software entitlement service native client library

This static library implements the client-side logic for verifying a software entitlement token provides a particular entitlement.  By default, it validates the connection to the server against a list of well-known long-lived Microsoft intermediate certificates, but supports additional certificates programatically.

**This is draft documentation subject to change.**

This project depends on two open-source packages: [OpenSSL](https://www.openssl.org/) and [libcurl](https://curl.haxx.se/libcurl/c/libcurl.html). (See below for details.)

## Windows build
The included project for Visual Studio 2017 depends libcurl and OpenSSL being available.  The simplest mechanism to achieve this is to use [vcpkg](https://github.com/Microsoft/vcpkg), following the Quick Start instructions, including user-wide integration.

## Building with Visual Studio 2012 or Visual Studio 2013
The libcurl and OpenSSL libraries expose a standard C interface, so can be built using Visual Studio 2017 and referenced in projects using earlier versions of Visual Studio without issue.  However, the vcpkg user-wide integration does not support versions prior to Visual Studio 2015, requiring the following project configuration settings to be modified:
* C/C++ | General | AdditionalIncludeDirectories: include the vcpkg include directory, e.g. 'D:\GitHub\vcpkg\installed\x64-windows\include'
* Linker | General | AdditionalLibraryDirectories: include the vcpkg lib directory, e.g. 'D:\GitHub\vcpkg\installed\x64-windows\lib'
* Linker | Input | AdditionalDependencies: include the following libs:
	* ssleay32.lib
	* libeay32.lib
	* libcurl_imp.lib

## Installing OpenSSL
For 32-bit builds:
```
> vcpkg install openssl
```

For 64-bit builds:
```
> vcpkg install openssl --triplet x64-windows
```

Both versions can be installed side-by-side if required.

## Installing libcurl
For 32-bit builds:
```
> vcpkg install curl
```

For 64-bit builds:
```
> vcpkg install curl --triplet x64-windows
```

Again, both versions can be installed side-by-side if required.

## Configuring libcurl to use OpenSSL
In order to validate an intermediate certificate in the server's certificate chain (not just the server's certificate), configure libcurl to use OpenSSL as the SSL library.  Note that for our usage of libcurl, we do not require ZLIB or LIBSSH2, so we remove those dependencies too.

In your vcpkg git repository clone, apply one of the following patches to make the required changes:

* For [curl 7.51.0-3](./curl-7.51.0-3.patch)
* For [curl 7.55.1-1](./curl-7.55.1-1.patch)

Now build and reinstall:

```
> vcpkg build curl [--triplet x64-Windows]
> vcpkg remove curl [--triplet x64-Windows]
> vcpkg install curl [--triplet x64-Windows]
```

## Usage
```
//
// The following initialization function must be invoked from the program's
// entry function (e.g. 'main'), as the library uses libcurl which has an
// absurd requirement that no other threads exist in the application when it
// is initialized.  See https://curl.haxx.se/libcurl/c/curl_global_init.html.
//
// Returns 0 if successful.
//
int err = Microsoft::Azure::Batch::SoftwareEntitlement::Init();
if (err != 0)
{
    ...
}

try
{
    //
    // Include the following call if you want to allow validating a server
    // connection using test certificates, passing the thumbprint and common
    // name of a certificate in the server's SSL certificate chain.  Remove it
    // for production releases: this will ensure that the code will only
    // authenticate to Azure Batch servers for token validation.
    //
    Microsoft::Azure::Batch::SoftwareEntitlement::AddSslCertificate(
        ssl_cert_thumbprint,
        ssl_cert_common_name
    );

    auto entitlement = Microsoft::Azure::Batch::SoftwareEntitlement::GetEntitlement(
        url,
        entitlement_token,
        requested_entitlement
    );
}
catch (const std::runtime_error& e)
{
    ...
}
//
// The entitlement can now be queried for its properties.
//
...

Microsoft::Azure::Batch::SoftwareEntitlement::Cleanup();
```

## Limitations
When calling ```AddSslCertificate```, you must not specify the thumbprint and common name of the root certificate of the server's SSL certificate chain.  This is because OpenSSL does not include the root certificate in the list of certificates.

## Attribution
This project depends on libcurl and OpenSSL.  As such, the following licenses apply and must be included in projects integrating this library:

| Open Source Project | Author | License URL |
| ------------------- | ------ | ----------- |
| [libcurl](http://curl.haxx.se) | [Daniel Stenberg](mailto:daniel@haxx.se) | <https://curl.haxx.se/docs/copyright.html> |
| [OpenSSL](http://www.openssl.org) | [OpenSSL Project](http://www.openssl.org/) | <https://www.openssl.org/source/license.html> |
| [JSON for Modern C++](https://github.com/nlohmann/json) | [Niels Lohmann](http://nlohmann.me) | <https://github.com/nlohmann/json/blob/develop/LICENSE.MIT> |

The OpenSSL license requires the following acknowledgements:

"This product includes software developed by the OpenSSL Project for use in the OpenSSL Toolkit (http://www.openssl.org)"  
"This product includes cryptographic software written by Eric Young (eay@cryptsoft.com)"

## Troubleshooting

### Cannot open include file: 'curl/curl.h'

Cannot open include file: 'curl/curl.h' may indicate that you don't have the appropriate version of [**libcurl**](https://curl.haxx.se/libcurl/c/libcurl.html) installed for your target platform. (This can happen if you install the x64 version of `libcurl` but then ask for an `x86` build.)

**Possible Solutions**: 1) Install `libcurl` for your target platform.
			2) Check that 'vcpkg' has been "integrated" correctly as per 	https://github.com/Microsoft/vcpkg/blob/master/README.md


