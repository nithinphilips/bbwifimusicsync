@echo off
mkdir out
curl --data @Test.Subscribe1.json -v http://localhost:9000/subscribe
curl --data @Test.Subscribe2.json -v http://localhost:9000/subscribe > out\Test.Subscribe.out.json
