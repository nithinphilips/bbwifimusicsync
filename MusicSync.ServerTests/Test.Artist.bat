@echo off
mkdir out
curl --data @Test.Artist.json -v http://localhost:9000/query > out\Test.Artist.out.json
