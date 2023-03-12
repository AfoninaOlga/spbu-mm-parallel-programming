import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.DisplayName;

import java.util.ArrayList;

import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.utbot.runtime.utils.java.UtUtils.deepEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

public final class MainTest {
    
    ///region Test suites for executable Main.computeLeadingNumbers

    ///region SYMBOLIC EXECUTION: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method computeLeadingNumbers(java.util.List, int, int)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#computeLeadingNumbers(java.util.List, int, int)}
     * @utbot.executesCondition {@code (allBoundaryNumbers == null): False}
     * @utbot.executesCondition {@code (allBoundaryNumbers.contains(null)): False}
     * @utbot.executesCondition {@code (rank < 0): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: rank < 0
     */
    @Test
    @DisplayName("computeLeadingNumbers: rank < 0 -> ThrowIllegalArgumentException")
    public void testComputeLeadingNumbers_RankLessThanZero() {
        ArrayList allBoundaryNumbers = new ArrayList();

        assertThrows(IllegalArgumentException.class, () -> Main.computeLeadingNumbers(allBoundaryNumbers, -1, -255));
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#computeLeadingNumbers(java.util.List, int, int)}
     * @utbot.executesCondition {@code (allBoundaryNumbers == null): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: allBoundaryNumbers == null
     */
    @Test
    @DisplayName("computeLeadingNumbers: allBoundaryNumbers == null -> ThrowIllegalArgumentException")
    public void testComputeLeadingNumbers_AllBoundaryNumbersEqualsNull() {
        assertThrows(IllegalArgumentException.class, () -> Main.computeLeadingNumbers(null, -255, -255));
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#computeLeadingNumbers(java.util.List, int, int)}
     * @utbot.executesCondition {@code (allBoundaryNumbers == null): False}
     * @utbot.executesCondition {@code (allBoundaryNumbers.contains(null)): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: allBoundaryNumbers.contains(null)
     */
    @Test
    @DisplayName("computeLeadingNumbers: allBoundaryNumbers.contains(null) -> ThrowIllegalArgumentException")
    public void testComputeLeadingNumbers_AllBoundaryNumbersContains() {
        ArrayList allBoundaryNumbers = new ArrayList();
        allBoundaryNumbers.add(null);
        allBoundaryNumbers.add(null);
        allBoundaryNumbers.add(null);

        assertThrows(IllegalArgumentException.class, () -> Main.computeLeadingNumbers(allBoundaryNumbers, -255, -255));
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#computeLeadingNumbers(java.util.List, int, int)}
     * @utbot.executesCondition {@code (allBoundaryNumbers == null): False}
     * @utbot.executesCondition {@code (allBoundaryNumbers.contains(null)): False}
     * @utbot.executesCondition {@code (rank < 0): False}
     * @utbot.executesCondition {@code (size < 1): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: size < 1
     */
    @Test
    @DisplayName("computeLeadingNumbers: size < 1 -> ThrowIllegalArgumentException")
    public void testComputeLeadingNumbers_SizeLessThan1() {
        ArrayList allBoundaryNumbers = new ArrayList();

        assertThrows(IllegalArgumentException.class, () -> Main.computeLeadingNumbers(allBoundaryNumbers, 0, 0));
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#computeLeadingNumbers(java.util.List, int, int)}
     * @utbot.executesCondition {@code (allBoundaryNumbers == null): False}
     * @utbot.executesCondition {@code (allBoundaryNumbers.contains(null)): False}
     * @utbot.executesCondition {@code (rank < 0): False}
     * @utbot.executesCondition {@code (size < 1): False}
     * @utbot.executesCondition {@code (rank >= size): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: rank >= size
     */
    @Test
    @DisplayName("computeLeadingNumbers: rank >= size -> ThrowIllegalArgumentException")
    public void testComputeLeadingNumbers_RankGreaterOrEqualSize() {
        ArrayList allBoundaryNumbers = new ArrayList();

        assertThrows(IllegalArgumentException.class, () -> Main.computeLeadingNumbers(allBoundaryNumbers, 128, 1));
    }
    ///endregion

    ///region OTHER: SUCCESSFUL EXECUTIONS for method computeLeadingNumbers(java.util.List, int, int)

    @Test
    public void testComputeLeadingNumbers1() {
        ArrayList allBoundaryNumbers = new ArrayList();

        ArrayList actual = ((ArrayList) Main.computeLeadingNumbers(allBoundaryNumbers, 0, 1));

        ArrayList expected = new ArrayList();
        assertTrue(deepEquals(expected, actual));
    }

    @Test
    public void testComputeLeadingNumbers2() {
        ArrayList allBoundaryNumbers = new ArrayList();

        ArrayList actual = ((ArrayList) Main.computeLeadingNumbers(allBoundaryNumbers, 2, 3));

        ArrayList expected = new ArrayList();
        assertTrue(deepEquals(expected, actual));
    }
    ///endregion

    ///endregion

    ///region Test suites for executable Main.fillInitialArray

    ///region SYMBOLIC EXECUTION: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method fillInitialArray(java.lang.String)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#fillInitialArray(String)}
     * @utbot.executesCondition {@code (fileName == null): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: fileName == null
     */
    @Test
    @DisplayName("fillInitialArray: fileName == null -> ThrowIllegalArgumentException")
    public void testFillInitialArray_FileNameEqualsNull() {
        assertThrows(IllegalArgumentException.class, () -> Main.fillInitialArray(null));
    }
    ///endregion

    ///region FUZZER: SECURITY for method fillInitialArray(java.lang.String)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#fillInitialArray(String)}
     */
    @Test
    @DisplayName("fillInitialArray: arg_0 = 'The file w...'")
    @org.junit.jupiter.api.Disabled(value = "Disabled due to sandbox")
    public void testFillInitialArrayWithNonEmptyString() {
        /* This test fails because method [Main.fillInitialArray] produces [java.security.AccessControlException: access denied ("java.io.FilePermission" "The file with the given fileName cannot be written to" "write")]
            java.base/java.security.AccessControlContext.checkPermission(AccessControlContext.java:485)
            java.base/java.security.AccessController.checkPermission(AccessController.java:1068)
            java.base/java.lang.SecurityManager.checkPermission(SecurityManager.java:416)
            java.base/java.lang.SecurityManager.checkWrite(SecurityManager.java:847)
            java.base/java.io.FileOutputStream.<init>(FileOutputStream.java:223)
            java.base/java.io.FileOutputStream.<init>(FileOutputStream.java:155)
            java.base/java.io.FileWriter.<init>(FileWriter.java:82)
            Main.fillInitialArray(Main.java:199) */
    }
    ///endregion

    ///region OTHER: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method fillInitialArray(java.lang.String)

    @Test
    public void testFillInitialArray1() {
        String fileName = "";

        assertThrows(IllegalArgumentException.class, () -> Main.fillInitialArray(fileName));
    }
    ///endregion

    ///endregion

    ///region Test suites for executable Main.getBoundaryNumbers

    ///region SYMBOLIC EXECUTION: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method getBoundaryNumbers(java.util.List, int)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getBoundaryNumbers(java.util.List, int)}
     * @utbot.executesCondition {@code (nodeArray == null): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: nodeArray == null
     */
    @Test
    @DisplayName("getBoundaryNumbers: nodeArray == null -> ThrowIllegalArgumentException")
    public void testGetBoundaryNumbers_NodeArrayEqualsNull() {
        assertThrows(IllegalArgumentException.class, () -> Main.getBoundaryNumbers(null, -255));
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getBoundaryNumbers(java.util.List, int)}
     * @utbot.executesCondition {@code (nodeArray == null): False}
     * @utbot.executesCondition {@code (size < 1): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: size < 1
     */
    @Test
    @DisplayName("getBoundaryNumbers: size < 1 -> ThrowIllegalArgumentException")
    public void testGetBoundaryNumbers_SizeLessThan1() {
        ArrayList nodeArray = new ArrayList();

        assertThrows(IllegalArgumentException.class, () -> Main.getBoundaryNumbers(nodeArray, 0));
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getBoundaryNumbers(java.util.List, int)}
     * @utbot.executesCondition {@code (nodeArray == null): False}
     * @utbot.executesCondition {@code (size < 1): False}
     * @utbot.executesCondition {@code (nodeArray.contains(null)): True}
     * @utbot.invokes {@link java.util.List#contains(Object)}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: nodeArray.contains(null)
     */
    @Test
    @DisplayName("getBoundaryNumbers: nodeArray.contains(null) -> ThrowIllegalArgumentException")
    public void testGetBoundaryNumbers_NodeArrayContains() {
        ArrayList nodeArray = new ArrayList();
        nodeArray.add(null);
        nodeArray.add(null);
        nodeArray.add(null);

        assertThrows(IllegalArgumentException.class, () -> Main.getBoundaryNumbers(nodeArray, 1));
    }
    ///endregion

    ///region OTHER: SUCCESSFUL EXECUTIONS for method getBoundaryNumbers(java.util.List, int)

    @Test
    public void testGetBoundaryNumbers1() {
        ArrayList nodeArray = new ArrayList();
        Integer integer = 0;
        nodeArray.add(integer);

        ArrayList actual = ((ArrayList) Main.getBoundaryNumbers(nodeArray, 1));

        ArrayList expected = new ArrayList();
        expected.add(integer);
        assertTrue(deepEquals(expected, actual));
    }

    @Test
    public void testGetBoundaryNumbers2() {
        ArrayList nodeArray = new ArrayList();

        ArrayList actual = ((ArrayList) Main.getBoundaryNumbers(nodeArray, 1));

        ArrayList expected = new ArrayList();
        assertTrue(deepEquals(expected, actual));
    }
    ///endregion

    ///endregion

    ///region Test suites for executable Main.getInitialArrayFromFile

    ///region SYMBOLIC EXECUTION: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method getInitialArrayFromFile(java.lang.String)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getInitialArrayFromFile(String)}
     * @utbot.executesCondition {@code (fileName == null): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: fileName == null
     */
    @Test
    @DisplayName("getInitialArrayFromFile: fileName == null -> ThrowIllegalArgumentException")
    public void testGetInitialArrayFromFile_FileNameEqualsNull() {
        assertThrows(IllegalArgumentException.class, () -> Main.getInitialArrayFromFile(null));
    }
    ///endregion

    ///region FUZZER: SECURITY for method getInitialArrayFromFile(java.lang.String)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getInitialArrayFromFile(String)}
     */
    @Test
    @DisplayName("getInitialArrayFromFile: arg_0 = 'File with ...'")
    @org.junit.jupiter.api.Disabled(value = "Disabled due to sandbox")
    public void testGetInitialArrayFromFileWithNonEmptyString() {
        /* This test fails because method [Main.getInitialArrayFromFile] produces [java.security.AccessControlException: access denied ("java.io.FilePermission" "File with the given fileName not found" "read")]
            java.base/java.security.AccessControlContext.checkPermission(AccessControlContext.java:485)
            java.base/java.security.AccessController.checkPermission(AccessController.java:1068)
            java.base/java.lang.SecurityManager.checkPermission(SecurityManager.java:416)
            java.base/java.lang.SecurityManager.checkRead(SecurityManager.java:756)
            java.base/java.io.FileInputStream.<init>(FileInputStream.java:146)
            java.base/java.util.Scanner.<init>(Scanner.java:639)
            Main.getInitialArrayFromFile(Main.java:335) */
    }
    ///endregion

    ///region OTHER: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method getInitialArrayFromFile(java.lang.String)

    @Test
    public void testGetInitialArrayFromFile1() {
        String fileName = "";

        assertThrows(IllegalArgumentException.class, () -> Main.getInitialArrayFromFile(fileName));
    }
    ///endregion

    ///endregion

    ///region Test suites for executable Main.getNodeBorders

    ///region SYMBOLIC EXECUTION: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method getNodeBorders(java.util.List, int)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getNodeBorders(java.util.List, int)}
     * @utbot.executesCondition {@code (initialArray == null): False}
     * @utbot.executesCondition {@code (size < 1): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: size < 1
     */
    @Test
    @DisplayName("getNodeBorders: size < 1 -> ThrowIllegalArgumentException")
    public void testGetNodeBorders_SizeLessThan1() {
        ArrayList initialArray = new ArrayList();

        assertThrows(IllegalArgumentException.class, () -> Main.getNodeBorders(initialArray, 0));
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getNodeBorders(java.util.List, int)}
     * @utbot.executesCondition {@code (initialArray == null): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: initialArray == null
     */
    @Test
    @DisplayName("getNodeBorders: initialArray == null -> ThrowIllegalArgumentException")
    public void testGetNodeBorders_InitialArrayEqualsNull() {
        assertThrows(IllegalArgumentException.class, () -> Main.getNodeBorders(null, -255));
    }
    ///endregion

    ///region OTHER: SUCCESSFUL EXECUTIONS for method getNodeBorders(java.util.List, int)

    @Test
    public void testGetNodeBorders1() {
        ArrayList initialArray = new ArrayList();
        initialArray.add(null);
        initialArray.add(null);
        initialArray.add(null);

        ArrayList actual = ((ArrayList) Main.getNodeBorders(initialArray, 1));

        ArrayList expected = new ArrayList();
        ArrayList arrayList = new ArrayList();
        Integer integer = 0;
        arrayList.add(integer);
        expected.add(arrayList);
        ArrayList arrayList1 = new ArrayList();
        Integer integer1 = 2;
        arrayList1.add(integer1);
        expected.add(arrayList1);
        assertTrue(deepEquals(expected, actual));
    }
    ///endregion

    ///endregion

    ///region Test suites for executable Main.getSentSequencesNumbers

    ///region SYMBOLIC EXECUTION: EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method getSentSequencesNumbers(java.util.List, java.util.List, int)

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getSentSequencesNumbers(java.util.List, java.util.List, int)}
     * @utbot.executesCondition {@code (nodeArray == null): False}
     * @utbot.executesCondition {@code (leadingNumbers == null): False}
     * @utbot.executesCondition {@code (size < 1): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: size < 1
     */
    @Test
    @DisplayName("getSentSequencesNumbers: size < 1 -> ThrowIllegalArgumentException")
    public void testGetSentSequencesNumbers_SizeLessThan1() {
        ArrayList nodeArray = new ArrayList();

        assertThrows(IllegalArgumentException.class, () -> Main.getSentSequencesNumbers(nodeArray, nodeArray, 0));
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getSentSequencesNumbers(java.util.List, java.util.List, int)}
     * @utbot.executesCondition {@code (nodeArray == null): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: nodeArray == null
     */
    @Test
    @DisplayName("getSentSequencesNumbers: nodeArray == null -> ThrowIllegalArgumentException")
    public void testGetSentSequencesNumbers_NodeArrayEqualsNull() {
        assertThrows(IllegalArgumentException.class, () -> Main.getSentSequencesNumbers(null, null, -255));
    }

    /**
     * @utbot.classUnderTest {@link Main}
     * @utbot.methodUnderTest {@link Main#getSentSequencesNumbers(java.util.List, java.util.List, int)}
     * @utbot.executesCondition {@code (nodeArray == null): False}
     * @utbot.executesCondition {@code (leadingNumbers == null): True}
     * @utbot.throwsException {@link IllegalArgumentException} after condition: leadingNumbers == null
     */
    @Test
    @DisplayName("getSentSequencesNumbers: leadingNumbers == null -> ThrowIllegalArgumentException")
    public void testGetSentSequencesNumbers_LeadingNumbersEqualsNull() {
        ArrayList nodeArray = new ArrayList();

        assertThrows(IllegalArgumentException.class, () -> Main.getSentSequencesNumbers(nodeArray, null, -255));
    }
    ///endregion

    ///region OTHER: SUCCESSFUL EXECUTIONS for method getSentSequencesNumbers(java.util.List, java.util.List, int)

    @Test
    public void testGetSentSequencesNumbers1() {
        ArrayList nodeArray = new ArrayList();
        nodeArray.add(null);
        nodeArray.add(null);
        nodeArray.add(null);
        ArrayList leadingNumbers = new ArrayList();

        ArrayList actual = ((ArrayList) Main.getSentSequencesNumbers(nodeArray, leadingNumbers, 1));

        ArrayList expected = new ArrayList();
        ArrayList arrayList = new ArrayList();
        arrayList.add(null);
        arrayList.add(null);
        arrayList.add(null);
        expected.add(arrayList);
        assertTrue(deepEquals(expected, actual));
    }

    @Test
    public void testGetSentSequencesNumbers2() {
        ArrayList nodeArray = new ArrayList();
        ArrayList leadingNumbers = new ArrayList();

        ArrayList actual = ((ArrayList) Main.getSentSequencesNumbers(nodeArray, leadingNumbers, 2));

        ArrayList expected = new ArrayList();
        ArrayList arrayList = new ArrayList();
        expected.add(arrayList);
        ArrayList arrayList1 = new ArrayList();
        expected.add(arrayList1);
        assertTrue(deepEquals(expected, actual));
    }
    ///endregion

    ///endregion

    ///region Test suites for executable Main.main

    ///region OTHER: SECURITY for method main(java.lang.String[])

    @Test
    @org.junit.jupiter.api.Disabled(value = "Disabled due to sandbox")
    public void testMain1() {
        /* This test fails because method [Main.main] produces [java.security.AccessControlException: access denied ("java.io.FilePermission" "..\resources\initialArray.txt" "read")]
            java.base/java.security.AccessControlContext.checkPermission(AccessControlContext.java:485)
            java.base/java.security.AccessController.checkPermission(AccessController.java:1068)
            java.base/java.lang.SecurityManager.checkPermission(SecurityManager.java:416)
            java.base/java.lang.SecurityManager.checkRead(SecurityManager.java:756)
            java.base/java.io.FileInputStream.<init>(FileInputStream.java:146)
            java.base/java.util.Scanner.<init>(Scanner.java:639)
            Main.getInitialArrayFromFile(Main.java:335)
            Main.main(Main.java:13) */
    }
    ///endregion

    ///endregion
}
