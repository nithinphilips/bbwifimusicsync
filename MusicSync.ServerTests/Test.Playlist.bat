@echo off
mkdir out
curl --data @Test.Playlist.json -v http://localhost:9000/query > out\Test.Playlist.out.json
