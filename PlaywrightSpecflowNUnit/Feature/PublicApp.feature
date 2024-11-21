Feature: PublicApp

A short summary of the feature

@tag1
Scenario: City Weather search and Validation
	Given User navigates to bbc weather page
	When User search for weather in "London" city
	Then Validate current temperature, wind speed and weather desciption are correct
