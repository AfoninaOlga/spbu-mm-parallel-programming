package main.java.quicksort;

import org.junit.Test;

import java.util.ArrayList;

import static org.utbot.runtime.utils.java.UtUtils.deepEquals;
import static org.junit.Assert.assertTrue;
import static org.junit.Assert.assertNull;

public final class MainTest {
    ///region Test suites for executable main.java.quicksort.Main.computeLeadingNumbers

    ///region SYMBOLIC EXECUTION: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method computeLeadingNumbers(java.util.List, int, int)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#computeLeadingNumbers(java.util.List, int, int)}
     * @utbot.executesCondition {@code (allBoundaryNumbers == null): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: allBoundaryNumbers == null
     */
    @Test(expected = IllegalArgumentException.class)
    public void testComputeLeadingNumbers_AllBoundaryNumbersEqualsNull() {
        Main.computeLeadingNumbers(null, -255, -255);
    }
    ///endregion

    ///region FUZZER: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method computeLeadingNumbers(java.util.List, int, int)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#computeLeadingNumbers(java.util.List, int, int)}
     */
    @Test(expected = IllegalArgumentException.class)
    public void testComputeLeadingNumbersThrowsIAEWithCornerCase() {
        ArrayList allBoundaryNumbers = new ArrayList();
        allBoundaryNumbers.add(-1);

        Main.computeLeadingNumbers(allBoundaryNumbers, -1, 0);
    }
    ///endregion

    ///region OTHER: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method computeLeadingNumbers(java.util.List, int, int)

    @Test(expected = IllegalArgumentException.class)
    public void testComputeLeadingNumbers1() {
        ArrayList allBoundaryNumbers = new ArrayList();
        allBoundaryNumbers.add(null);
        allBoundaryNumbers.add(null);
        allBoundaryNumbers.add(null);

        Main.computeLeadingNumbers(allBoundaryNumbers, 0, 0);
    }
    ///endregion

    ///endregion

    ///region Test suites for executable main.java.quicksort.Main.fillInitialArray

    ///region SYMBOLIC EXECUTION: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method fillInitialArray(java.lang.String)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#fillInitialArray(String)}
     * @utbot.executesCondition {@code (fileName == null): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: fileName == null
     */
    @Test(expected = IllegalArgumentException.class)
    public void testFillInitialArray_FileNameEqualsNull() {
        Main.fillInitialArray(null);
    }
    ///endregion

    ///region FUZZER: SECURITY for method fillInitialArray(java.lang.String)

    ///region OTHER: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method fillInitialArray(java.lang.String)

    @Test(expected = IllegalArgumentException.class)
    public void testFillInitialArray1() {
        String fileName = "";

        Main.fillInitialArray(fileName);
    }
    ///endregion

    ///endregion

    ///region Test suites for executable main.java.quicksort.Main.getBoundaryNumbers

    ///region SYMBOLIC EXECUTION: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method getBoundaryNumbers(java.util.List, int)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getBoundaryNumbers(java.util.List, int)}
     * @utbot.executesCondition {@code (nodeArray == null): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: nodeArray == null
     */
    @Test(expected = IllegalArgumentException.class)
    public void testGetBoundaryNumbers_NodeArrayEqualsNull() {
        Main.getBoundaryNumbers(null, -255);
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getBoundaryNumbers(java.util.List, int)}
     * @utbot.executesCondition {@code (nodeArray == null): False}
     * @utbot.executesCondition {@code (size < 1): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: size < 1
     */
    @Test(expected = IllegalArgumentException.class)
    public void testGetBoundaryNumbers_SizeLessThan1() {
        ArrayList nodeArray = new ArrayList();

        Main.getBoundaryNumbers(nodeArray, 0);
    }
    ///endregion

    ///region FUZZER: SUCCESSFUL EXECUTIONS for method getBoundaryNumbers(java.util.List, int)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getBoundaryNumbers(java.util.List, int)}
     */
    @Test
    public void testGetBoundaryNumbers() {
        ArrayList nodeArray = new ArrayList();
        nodeArray.add(0);

        ArrayList actual = ((ArrayList) Main.getBoundaryNumbers(nodeArray, 1));

        ArrayList expected = new ArrayList();
        Integer integer = 0;
        expected.add(integer);
        assertTrue(deepEquals(expected, actual));
    }
    ///endregion

    ///region OTHER: SUCCESSFUL EXECUTIONS for method getBoundaryNumbers(java.util.List, int)

    @Test
    public void testGetBoundaryNumbers1() {
        ArrayList nodeArray = new ArrayList();

        ArrayList actual = ((ArrayList) Main.getBoundaryNumbers(nodeArray, 1));

        ArrayList expected = new ArrayList();
        assertTrue(deepEquals(expected, actual));
    }
    ///endregion

    ///endregion

    ///region Test suites for executable main.java.quicksort.Main.getFileNames

    ///region SYMBOLIC EXECUTION: SUCCESSFUL EXECUTIONS for method getFileNames(java.lang.String[])

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getFileNames(String[])}
     * @utbot.executesCondition {@code (args == null): False}
     * @utbot.executesCondition {@code (args.length < 4): False}
     * @utbot.executesCondition {@code (args.length < 5): False}
     * @utbot.invokes {@link java.util.List#add(Object)}
     * @utbot.invokes {@link java.util.List#add(Object)}
     * @utbot.returnsFrom {@code return fileNames;}
     */
    @Test
    public void testGetFileNames_ArgsLengthGreaterOrEqual5() {
        String[] args = new String[13];

        ArrayList actual = ((ArrayList) Main.getFileNames(args));

        ArrayList expected = new ArrayList();
        expected.add(null);
        expected.add(null);
        assertTrue(deepEquals(expected, actual));

        String finalArgs0 = args[0];
        String finalArgs1 = args[1];
        String finalArgs2 = args[2];
        String finalArgs3 = args[3];
        String finalArgs4 = args[4];
        String finalArgs5 = args[5];
        String finalArgs6 = args[6];
        String finalArgs7 = args[7];
        String finalArgs8 = args[8];
        String finalArgs9 = args[9];
        String finalArgs10 = args[10];
        String finalArgs11 = args[11];
        String finalArgs12 = args[12];

        assertNull(finalArgs0);

        assertNull(finalArgs1);

        assertNull(finalArgs2);

        assertNull(finalArgs3);

        assertNull(finalArgs4);

        assertNull(finalArgs5);

        assertNull(finalArgs6);

        assertNull(finalArgs7);

        assertNull(finalArgs8);

        assertNull(finalArgs9);

        assertNull(finalArgs10);

        assertNull(finalArgs11);

        assertNull(finalArgs12);
    }
    ///endregion

    ///region SYMBOLIC EXECUTION: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method getFileNames(java.lang.String[])

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getFileNames(String[])}
     * @utbot.executesCondition {@code (args == null): False}
     * @utbot.executesCondition {@code (args.length < 4): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: args.length < 4
     */
    @Test(expected = IllegalArgumentException.class)
    public void testGetFileNames_ArgsLengthLessThan4() {
        String[] args = {null};

        Main.getFileNames(args);
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getFileNames(String[])}
     * @utbot.executesCondition {@code (args == null): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: args == null
     */
    @Test(expected = IllegalArgumentException.class)
    public void testGetFileNames_ArgsEqualsNull() {
        Main.getFileNames(null);
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getFileNames(String[])}
     * @utbot.executesCondition {@code (args == null): False}
     * @utbot.executesCondition {@code (args.length < 4): False}
     * @utbot.executesCondition {@code (args.length < 5): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: args.length < 5
     */
    @Test(expected = IllegalArgumentException.class)
    public void testGetFileNames_ArgsLengthLessThan5() {
        String[] args = {null, null, null, null};

        Main.getFileNames(args);
    }
    ///endregion

    ///endregion

    ///region Test suites for executable main.java.quicksort.Main.getInitialArrayFromFile

    ///region SYMBOLIC EXECUTION: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method getInitialArrayFromFile(java.lang.String)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getInitialArrayFromFile(String)}
     * @utbot.executesCondition {@code (fileName == null): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: fileName == null
     */
    @Test(expected = IllegalArgumentException.class)
    public void testGetInitialArrayFromFile_FileNameEqualsNull() {
        Main.getInitialArrayFromFile(null);
    }
    ///endregion

    ///region FUZZER: SECURITY for method getInitialArrayFromFile(java.lang.String)

    ///region OTHER: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method getInitialArrayFromFile(java.lang.String)

    @Test(expected = IllegalArgumentException.class)
    public void testGetInitialArrayFromFile1() {
        String fileName = "";

        Main.getInitialArrayFromFile(fileName);
    }
    ///endregion

    ///endregion

    ///region Test suites for executable main.java.quicksort.Main.getNodeBorders

    ///region SYMBOLIC EXECUTION: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method getNodeBorders(java.util.List, int)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getNodeBorders(java.util.List, int)}
     * @utbot.executesCondition {@code (initialArray == null): False}
     * @utbot.executesCondition {@code (size < 1): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: size < 1
     */
    @Test(expected = IllegalArgumentException.class)
    public void testGetNodeBorders_SizeLessThan1() {
        ArrayList initialArray = new ArrayList();

        Main.getNodeBorders(initialArray, 0);
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getNodeBorders(java.util.List, int)}
     * @utbot.executesCondition {@code (initialArray == null): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: initialArray == null
     */
    @Test(expected = IllegalArgumentException.class)
    public void testGetNodeBorders_InitialArrayEqualsNull() {
        Main.getNodeBorders(null, -255);
    }
    ///endregion

    ///endregion

    ///region Test suites for executable main.java.quicksort.Main.getSentSequencesNumbers

    ///region SYMBOLIC EXECUTION: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method getSentSequencesNumbers(java.util.List, java.util.List, int)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getSentSequencesNumbers(java.util.List, java.util.List, int)}
     * @utbot.executesCondition {@code (nodeArray == null): False}
     * @utbot.executesCondition {@code (leadingNumbers == null): False}
     * @utbot.executesCondition {@code (size < 1): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: size < 1
     */
    @Test(expected = IllegalArgumentException.class)
    public void testGetSentSequencesNumbers_SizeLessThan1() {
        ArrayList nodeArray = new ArrayList();

        Main.getSentSequencesNumbers(nodeArray, nodeArray, 0);
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getSentSequencesNumbers(java.util.List, java.util.List, int)}
     * @utbot.executesCondition {@code (nodeArray == null): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: nodeArray == null
     */
    @Test(expected = IllegalArgumentException.class)
    public void testGetSentSequencesNumbers_NodeArrayEqualsNull() {
        Main.getSentSequencesNumbers(null, null, -255);
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getSentSequencesNumbers(java.util.List, java.util.List, int)}
     * @utbot.executesCondition {@code (nodeArray == null): False}
     * @utbot.executesCondition {@code (leadingNumbers == null): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: leadingNumbers == null
     */
    @Test(expected = IllegalArgumentException.class)
    public void testGetSentSequencesNumbers_LeadingNumbersEqualsNull() {
        ArrayList nodeArray = new ArrayList();

        Main.getSentSequencesNumbers(nodeArray, null, -255);
    }
    ///endregion

    ///endregion

    ///region Test suites for executable main.java.quicksort.Main.main

    ///region SYMBOLIC EXECUTION: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method main(java.lang.String[])

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#main(String[])}
     * @utbot.executesCondition {@code (args == null): False}
     * @utbot.executesCondition {@code (args.length < 4): True}
     * @utbot.invokes {@link Main#getFileNames(String[])}
     * @utbot.throwsException {@link IllegalArgumentException} in: List<String> fileNames = getFileNames(args);
     */
    @Test(expected = IllegalArgumentException.class)
    public void testMain_ArgsLengthLessThan4() {
        String[] args = {};

        Main.main(args);
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#main(String[])}
     * @utbot.executesCondition {@code (args == null): True}
     * @utbot.invokes {@link Main#getFileNames(String[])}
     * @utbot.throwsException {@link IllegalArgumentException} in: List<String> fileNames = getFileNames(args);
     */
    @Test(expected = IllegalArgumentException.class)
    public void testMain_ArgsEqualsNull() {
        Main.main(null);
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#main(String[])}
     * @utbot.executesCondition {@code (args == null): False}
     * @utbot.executesCondition {@code (args.length < 4): False}
     * @utbot.executesCondition {@code (args.length < 5): True}
     * @utbot.invokes {@link Main#getFileNames(String[])}
     * @utbot.throwsException {@link IllegalArgumentException} in: List<String> fileNames = getFileNames(args);
     */
    @Test(expected = IllegalArgumentException.class)
    public void testMain_ArgsLengthLessThan5() {
        String[] args = {null, null, null, null};

        Main.main(args);
    }
    ///endregion

    ///region OTHER: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method main(java.lang.String[])

    @Test(expected = IllegalArgumentException.class)
    public void testMain1() {
        String[] args = new String[12];

        Main.main(args);
    }
    ///endregion

    ///endregion

    ///region Test suites for executable main.java.quicksort.Main.printSortedArrayToFile

    ///region FUZZER: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method printSortedArrayToFile(java.lang.String, java.util.List, int, int)

    @Test(expected = IllegalArgumentException.class)
    public void testPrintSortedArrayToFileByFuzzer() {
        ArrayList array = new ArrayList();

        Main.printSortedArrayToFile("The given array must not be null", array, 1, 1);
    }
    ///endregion

    ///region OTHER: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method printSortedArrayToFile(java.lang.String, java.util.List, int, int)

    @Test(expected = IllegalArgumentException.class)
    public void testPrintSortedArrayToFile1() {
        String fileName = "";

        Main.printSortedArrayToFile(fileName, null, 0, 0);
    }
    ///endregion

    ///endregion
}
