# $1=host url to add to

for i in $(seq 1 10)
do
  for j in $(seq 1 10)
  do
    curl -X POST "$1?studentId=$i&courseId=$j"
  done
done
