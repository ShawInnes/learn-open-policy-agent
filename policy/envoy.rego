package envoy.authz

import rego.v1

import input.attributes.request.http

default allow := false

allow if  {
    http.method == "GET"
    http.path == "/hello/world"
}

test_permit if {
	allow with input as {"attributes": {"request": {"http": {"method": "GET", "path": "/hello/world"}}}}
}

test_fail_method if {
	not allow with input as {"attributes": {"request": {"http": {"method": "POST", "path": "/hello/world"}}}}
}

test_fail_path if {
	not allow with input as {"attributes": {"request": {"http": {"method": "GET", "path": "/hello/fails"}}}}
}
