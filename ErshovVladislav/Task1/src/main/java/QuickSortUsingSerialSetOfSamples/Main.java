package QuickSortUsingSerialSetOfSamples;

import mpi.MPI;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileWriter;
import java.io.IOException;
import java.util.*;
import java.util.concurrent.ThreadLocalRandom;

public class Main {

    private final static String FILE_NAME_WITH_INITIAL_ARRAY = "../resources/initialArray.txt"; //$NON-NLS-1$

    public static void main(String[] args) {
        // fillInitialArray(FILE_NAME_WITH_INITIAL_ARRAY);
        List<Integer> initialArray = getInitialArrayFromFile(FILE_NAME_WITH_INITIAL_ARRAY);

        MPI.Init(args);
        int rank = MPI.COMM_WORLD.Rank();
        int size = MPI.COMM_WORLD.Size();

        long time = System.currentTimeMillis();

        // Prepare
        List<List<Integer>> nodeBorders = getNodeBorders(initialArray, size);
        List<Integer> leftBorders = nodeBorders.get(0);
        List<Integer> rightBorders = nodeBorders.get(1);

        List<Integer> nodeArray = initialArray.subList(leftBorders.get(rank), rightBorders.get(rank) + 1);

        // 1 phase
        Collections.sort(nodeArray);

        List<Integer> boundaryNumbers = getBoundaryNumbers(nodeArray, size);

        // Send boundary numbers to main node after 1 phase
        if (rank != 0) {
            int[] sendBufferBoundaryNumbersSize = { boundaryNumbers.size() };
            MPI.COMM_WORLD.Send(sendBufferBoundaryNumbersSize, 0, 1, MPI.INT, 0, 11);

            int[] sendBufferBoundaryNumbers = new int[sendBufferBoundaryNumbersSize[0]];
            for (int i = 0; i < sendBufferBoundaryNumbersSize[0]; i++) {
                sendBufferBoundaryNumbers[i] = boundaryNumbers.get(i);
            }
            MPI.COMM_WORLD.Send(sendBufferBoundaryNumbers, 0, sendBufferBoundaryNumbersSize[0], MPI.INT, 0, 12);
        }

        // 2 phase
        List<Integer> allBoundaryNumbers = new ArrayList<>();

        // Receive boundary numbers on main node after 1 phase
        if (rank == 0) {
            int[] receiveBufferBoundaryNumbersSize = new int[size];
            for (int i = 1; i < size; i++) {
                MPI.COMM_WORLD.Recv(receiveBufferBoundaryNumbersSize, i, 1, MPI.INT, i, 11);
            }

            for (int i = 1; i < size; i++) {
                int[] receiveBufferBoundaryNumbers = new int[receiveBufferBoundaryNumbersSize[i]];
                MPI.COMM_WORLD.Recv(receiveBufferBoundaryNumbers, 0, receiveBufferBoundaryNumbersSize[i], MPI.INT, i,
                        12);

                for (int number : receiveBufferBoundaryNumbers) {
                    allBoundaryNumbers.add(number);
                }
            }

            allBoundaryNumbers.addAll(boundaryNumbers);
        }

        Collections.sort(allBoundaryNumbers);

        List<Integer> leadingNumbers = computeLeadingNumbers(allBoundaryNumbers, rank, size);

        // Send leading numbers from main node to all other nodes after 2 phase
        if (rank == 0) {
            int leadingNumbersSize = leadingNumbers.size();

            int[] sendBufferLeadingNumbersSize = new int[] { leadingNumbersSize };
            for (int i = 1; i < size; i++) {
                MPI.COMM_WORLD.Send(sendBufferLeadingNumbersSize, 0, 1, MPI.INT, i, 21);
            }

            int[] sendBufferLeadingNumbers = new int[leadingNumbersSize];
            for (int i = 0; i < leadingNumbersSize; i++) {
                sendBufferLeadingNumbers[i] = leadingNumbers.get(i);
            }

            for (int i = 1; i < size; i++) {
                MPI.COMM_WORLD.Send(sendBufferLeadingNumbers, 0, leadingNumbersSize, MPI.INT, i, 22);
            }
        }

        // 3 phase

        // Receive leading numbers on all node after 2 phase
        if (rank != 0) {
            int[] receiveBufferLeadingNumbersSize = new int[1];
            MPI.COMM_WORLD.Recv(receiveBufferLeadingNumbersSize, 0, 1, MPI.INT, 0, 21);

            int[] receiveBufferLeadingNumbers = new int[receiveBufferLeadingNumbersSize[0]];
            MPI.COMM_WORLD.Recv(receiveBufferLeadingNumbers, 0, receiveBufferLeadingNumbersSize[0], MPI.INT, 0, 22);

            for (int number : receiveBufferLeadingNumbers) {
                leadingNumbers.add(number);
            }
        }

        List<List<Integer>> sentSequencesNumbers = getSentSequencesNumbers(nodeArray, leadingNumbers, size);

        List<Integer> receiveSequenceNumbersSizes = new ArrayList<>();
        for (int i = 0; i < size; i++) {
            if (rank < i) {
                int[] sendBufferSequenceNumbersSize = { sentSequencesNumbers.get(i).size() };
                MPI.COMM_WORLD.Send(sendBufferSequenceNumbersSize, 0, 1, MPI.INT, i, 31);

                int[] receiveBufferSequenceNumbersSize = new int[1];
                MPI.COMM_WORLD.Recv(receiveBufferSequenceNumbersSize, 0, 1, MPI.INT, i, 31);
                receiveSequenceNumbersSizes.add(receiveBufferSequenceNumbersSize[0]);
            } else if (rank > i) {
                int[] receiveBufferSequenceNumbersSize = new int[1];
                MPI.COMM_WORLD.Recv(receiveBufferSequenceNumbersSize, 0, 1, MPI.INT, i, 31);
                receiveSequenceNumbersSizes.add(receiveBufferSequenceNumbersSize[0]);

                int[] sendBufferSequenceNumbersSize = { sentSequencesNumbers.get(i).size() };
                MPI.COMM_WORLD.Send(sendBufferSequenceNumbersSize, 0, 1, MPI.INT, i, 31);
            } else {
                receiveSequenceNumbersSizes.add(0);
            }
        }

        List<List<Integer>> receiveSequencesNumbers = new ArrayList<>();
        for (int i = 0; i < size; i++) {
            receiveSequencesNumbers.add(new ArrayList<>());
        }

        for (int i = 0; i < size; i++) {
            if (rank < i) {
                int[] sendBufferSequenceNumbers = new int[sentSequencesNumbers.get(i).size()];
                for (int j = 0; j < sentSequencesNumbers.get(i).size(); j++) {
                    sendBufferSequenceNumbers[j] = sentSequencesNumbers.get(i).get(j);
                }
                MPI.COMM_WORLD.Send(sendBufferSequenceNumbers, 0, sentSequencesNumbers.get(i).size(), MPI.INT, i, 32);

                int[] receiveBufferSequenceNumbers = new int[receiveSequenceNumbersSizes.get(i)];
                MPI.COMM_WORLD.Recv(receiveBufferSequenceNumbers, 0, receiveSequenceNumbersSizes.get(i), MPI.INT, i,
                        32);

                for (int number : receiveBufferSequenceNumbers) {
                    receiveSequencesNumbers.get(i).add(number);
                }
            } else if (rank > i) {
                int[] receiveBufferSequenceNumbers = new int[receiveSequenceNumbersSizes.get(i)];
                MPI.COMM_WORLD.Recv(receiveBufferSequenceNumbers, 0, receiveSequenceNumbersSizes.get(i), MPI.INT, i,
                        32);

                for (int number : receiveBufferSequenceNumbers) {
                    receiveSequencesNumbers.get(i).add(number);
                }

                int[] sendBufferSequenceNumbers = new int[sentSequencesNumbers.get(i).size()];
                for (int j = 0; j < sentSequencesNumbers.get(i).size(); j++) {
                    sendBufferSequenceNumbers[j] = sentSequencesNumbers.get(i).get(j);
                }
                MPI.COMM_WORLD.Send(sendBufferSequenceNumbers, 0, sentSequencesNumbers.get(i).size(), MPI.INT, i, 32);
            } else {
                receiveSequencesNumbers.get(i).addAll(sentSequencesNumbers.get(i));
            }
        }

        List<Integer> finalNodeNumbers = new ArrayList<>();
        for (List<Integer> receiveSequenceNumbers : receiveSequencesNumbers) {
            finalNodeNumbers.addAll(receiveSequenceNumbers);
        }

        // 4 phase
        Collections.sort(finalNodeNumbers);

        double finalTime = ((double) (System.currentTimeMillis() - time)) / 100;

        // Print sorted array
        printNodeArray("Answer:", finalNodeNumbers, rank, size); //$NON-NLS-1$

        if (rank == 0) {
            System.out.println("Size: " + size); //$NON-NLS-1$
            System.out.println("Full time without read initialArray and print answer:"); //$NON-NLS-1$
            System.out.println(finalTime);
        }

        MPI.Finalize();
    }

    /**
     * Fill initial array in file.
     *
     * @param fileName file name
     * @throws IllegalArgumentException if fileName is null, blank or empty or the
     *                                  file with the given fileName cannot be
     *                                  written to
     */
    public static void fillInitialArray(String fileName) throws IllegalArgumentException {
        if (fileName == null) {
            throw new IllegalArgumentException("The given fileName must not be null"); //$NON-NLS-1$
        } else if (fileName.isBlank() || fileName.isEmpty()) {
            throw new IllegalArgumentException("The given fileName must not be blank or empty"); //$NON-NLS-1$
        }

        try (FileWriter writer = new FileWriter(fileName, false)) {
            int randomNum;
            for (int i = 0; i < 10000000; i++) {
                randomNum = ThreadLocalRandom.current().nextInt(0, 1000000 + 1);
                writer.write(randomNum + " "); //$NON-NLS-1$
            }
            writer.flush();
        } catch (IOException ex) {
            throw new IllegalArgumentException("The file with the given fileName cannot be written to"); //$NON-NLS-1$
        }
    }

    /**
     * Get boundary numbers according to the algorithm.
     *
     * @param nodeArray target array
     * @param size      number of nodes
     * @return list of boundary numbers
     * @throws IllegalArgumentException if nodeArray is null, nodeArray contain null
     *                                  values or size < 1
     */
    public static List<Integer> getBoundaryNumbers(List<Integer> nodeArray, int size) throws IllegalArgumentException {
        if (nodeArray == null) {
            throw new IllegalArgumentException("The given nodeArray must not be null"); //$NON-NLS-1$
        } else if (size < 1) {
            throw new IllegalArgumentException("The given size must be greater than one"); //$NON-NLS-1$
        } else if (nodeArray.contains(null)) {
            throw new IllegalArgumentException("In the given nodeArray must not contain null values"); //$NON-NLS-1$
        } else if (nodeArray.isEmpty()) {
            return new ArrayList<>();
        } else if (nodeArray.size() < size) {
            return new ArrayList<>(List.of(new Integer[] { nodeArray.get(0) }));
        }

        List<Integer> boundaryNumbers = new ArrayList<>();
        int i = 0;
        while (i * (nodeArray.size() / size) < nodeArray.size()) {
            boundaryNumbers.add(nodeArray.get(i * (nodeArray.size() / size)));
            i++;
        }

        return boundaryNumbers;
    }

    /**
     * Compute leading numbers. Usefully only on main node.
     *
     * @param allBoundaryNumbers all boundary numbers from all nodes
     * @param rank               node number
     * @param size               number of nodes
     * @return list of leading numbers
     * @throws IllegalArgumentException if allBoundaryNumbers is null,
     *                                  allBoundaryNumbers contain null values, size
     *                                  < 1, rank < 0 or rank >= size
     */
    public static List<Integer> computeLeadingNumbers(List<Integer> allBoundaryNumbers, int rank, int size)
            throws IllegalArgumentException {
        if (allBoundaryNumbers == null) {
            throw new IllegalArgumentException("The given array must not be null"); //$NON-NLS-1$
        } else if (allBoundaryNumbers.contains(null)) {
            throw new IllegalArgumentException("In the given allBoundaryNumbers must not contain null values"); //$NON-NLS-1$
        } else if (rank < 0) {
            throw new IllegalArgumentException("The given rank must be greater than zero"); //$NON-NLS-1$
        } else if (size < 1) {
            throw new IllegalArgumentException("The given size must be greater than one"); //$NON-NLS-1$
        } else if (rank >= size) {
            throw new IllegalArgumentException("The given rank must be less than given size"); //$NON-NLS-1$
        } else if (rank != 0) {
            return new ArrayList<>();
        }

        List<Integer> leadingNumbers = new ArrayList<>();
        for (int i = 0; i < size - 1; i++) {
            int index = (i + 1) * size + size / 2 - 1;
            if (index > allBoundaryNumbers.size() - 1 || index < 0)
                break;
            leadingNumbers.add(allBoundaryNumbers.get(index));
        }
        return leadingNumbers;
    }

    /**
     * Get left and right node array borders from initial array.
     *
     * @param initialArray initial array
     * @param size         number of nodes
     * @return list of left and right borders
     * @throws IllegalArgumentException if initialArray is null or size < 1
     */
    public static List<List<Integer>> getNodeBorders(List<Integer> initialArray, int size)
            throws IllegalArgumentException {
        if (initialArray == null) {
            throw new IllegalArgumentException("The given initialArray must not be null"); //$NON-NLS-1$
        } else if (size < 1) {
            throw new IllegalArgumentException("The given size must be greater than one"); //$NON-NLS-1$
        }

        List<Integer> leftBorders = new ArrayList<>();
        List<Integer> rightBorders = new ArrayList<>();

        int nodeArrayLength = initialArray.size() / size;
        for (int i = 0; i < size; i++) {
            leftBorders.add(i * nodeArrayLength);
            if (i < size - 1) {
                rightBorders.add((i + 1) * nodeArrayLength - 1);
            } else {
                rightBorders.add(initialArray.size() - 1);
            }
        }

        List<List<Integer>> nodeBorders = new ArrayList<>();
        nodeBorders.add(leftBorders);
        nodeBorders.add(rightBorders);

        return nodeBorders;
    }

    /**
     * Get initial array from file.
     *
     * @param fileName file name
     * @return initial array
     * @throws IllegalArgumentException if fileName is null, blank or empty or file
     *                                  with the given fileName not found
     */
    public static List<Integer> getInitialArrayFromFile(String fileName) throws IllegalArgumentException {
        if (fileName == null) {
            throw new IllegalArgumentException("The given fileName must not be null"); //$NON-NLS-1$
        } else if (fileName.isBlank() || fileName.isEmpty()) {
            throw new IllegalArgumentException("The given fileName must not be blank or empty"); //$NON-NLS-1$
        }

        List<Integer> initialArray = new ArrayList<>();

        File file = new File(fileName);
        try (Scanner scanner = new Scanner(file)) {
            while (scanner.hasNextInt()) {
                initialArray.add(scanner.nextInt());
            }
        } catch (FileNotFoundException ex) {
            throw new IllegalArgumentException("File with the given fileName not found"); //$NON-NLS-1$
        }

        return initialArray;
    }

    /**
     * Get sequences of numbers to sent from node to other nodes.
     *
     * @param nodeArray      array on node
     * @param leadingNumbers leading numbers
     * @param size           number of nodes
     * @return list of sequences of numbers
     * @throws IllegalArgumentException if nodeArray or leadingNumbers is null, size
     *                                  < 1
     */
    public static List<List<Integer>> getSentSequencesNumbers(List<Integer> nodeArray, List<Integer> leadingNumbers,
                                                              int size) throws IllegalArgumentException {
        if (nodeArray == null) {
            throw new IllegalArgumentException("The given nodeArray must not be null"); //$NON-NLS-1$
        } else if (leadingNumbers == null) {
            throw new IllegalArgumentException("The given leadingNumbers must not be null"); //$NON-NLS-1$
        } else if (size < 1) {
            throw new IllegalArgumentException("The given size must be greater than one"); //$NON-NLS-1$
        }

        List<List<Integer>> sentSequencesNumbers = new ArrayList<>();
        for (int i = 0; i < size; i++) {
            sentSequencesNumbers.add(new ArrayList<>());
        }

        int j = 0;
        for (Integer number : nodeArray) {
            while (j < leadingNumbers.size() && number > leadingNumbers.get(j)) {
                j++;
            }
            sentSequencesNumbers.get(j).add(number);
        }

        return sentSequencesNumbers;
    }

    /**
     * Print array on node.
     *
     * @param title title of array
     * @param array array on node
     * @param rank  node number
     * @param size  number of nodes
     * @throws IllegalArgumentException if title is null, array is null, size < 1,
     *                                  rank < 0 or rank >= size
     */
    private static void printNodeArray(String title, List<Integer> array, int rank, int size)
            throws IllegalArgumentException {
        if (title == null) {
            throw new IllegalArgumentException("The given title must not be null"); //$NON-NLS-1$
        } else if (array == null) {
            throw new IllegalArgumentException("The given array must not be null"); //$NON-NLS-1$
        } else if (rank < 0) {
            throw new IllegalArgumentException("The given rank must be greater than zero"); //$NON-NLS-1$
        } else if (size < 1) {
            throw new IllegalArgumentException("The given size must be greater than one"); //$NON-NLS-1$
        } else if (rank >= size) {
            throw new IllegalArgumentException("The given rank must be less than given size"); //$NON-NLS-1$
        }

        if (rank == 0) {
            System.out.println(title);
        }

        for (int i = 0; i < size; i++) {
            if (rank == i) {
                System.out.println("Rank: " + rank); //$NON-NLS-1$
                System.out.println(Arrays.toString(array.toArray()));
            }

            MPI.COMM_WORLD.Barrier();
        }
    }

}
