Feature: Booking API testig

A short summary of the feature

Background:
	Given I have initialized the API context

@api
Scenario: Get the list of bookings
	When I request the list of bookings
	Then the response should contain a list of bookings
