# $1=path to jmeter exec

jmeter=$1

# Load test fine-grained
container_hash=$(docker run -p 8080:8080 -d exam-system)
sleep 10
$jmeter -n -t "test_plans/Load Test 4000 users 10 loops 10^3 ids (fine-grained).jmx"
docker stop "$container_hash" && docker rm "$container_hash"

# Load test lazy
container_hash=$(docker run -p 8080:8080 -d exam-system)
sleep 10
$jmeter -n -t "test_plans/Load Test 4000 users 10 loops 10^3 ids (lazy).jmx"
docker stop "$container_hash" && docker rm "$container_hash"

for system in lazy fine-grained
do
  for rps in 100 500
  do
    container_hash=$(docker run -p 8080:8080 -d exam-system)
    sleep 10
    source pre-populate100.sh "http://0.0.0.0:8080/$system"
    $jmeter -n -t "test_plans/Add load $(rps)rps ($system).jmx"
    docker stop "$container_hash" && docker rm "$container_hash"

    container_hash=$(docker run -p 8080:8080 -d exam-system)
    sleep 10
    source pre-populate100.sh "http://0.0.0.0:8080/$system"
    $jmeter -n -t "test_plans/Contains load $(rps)rps ($system).jmx"
    docker stop "$container_hash" && docker rm "$container_hash"

    container_hash=$(docker run -p 8080:8080 -d exam-system)
    sleep 10
    source pre-populate4900.sh "http://0.0.0.0:8080/$system"
    $jmeter -n -t "test_plans/Remove load $(rps)rps ($system).jmx"
    docker stop "$container_hash" && docker rm "$container_hash"
  done
done
