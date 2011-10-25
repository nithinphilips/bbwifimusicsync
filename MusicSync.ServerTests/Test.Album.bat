@echo off
mkdir out
curl --data @Test.Album.json -v http://localhost:9000/query > out\Test.Album.out.json
