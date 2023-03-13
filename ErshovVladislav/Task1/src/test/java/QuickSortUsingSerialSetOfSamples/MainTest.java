package QuickSortUsingSerialSetOfSamples;

import org.junit.Test;
import java.util.LinkedList;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import java.util.Map;
import java.util.Set;
import java.util.HashSet;
import java.lang.reflect.Field;
import java.util.Arrays;
import java.lang.reflect.Array;
import java.util.Iterator;
import java.util.stream.Stream;

import static org.junit.Assert.assertTrue;
import static java.util.Collections.emptyList;
import static java.util.Collections.synchronizedList;

public class MainTest {
    ///region Test suites for executable main.java.Main.main
    
    ///region
    
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("main: arg_0 = String[0] -> throw IllegalArgumentException")
    public void testMainThrowsIAEWithEmptyObjectArray() {
        java.lang.String[] args = {};
        
        Main.main(args);
    }
    ///endregion
    
    ///endregion
    
    ///region Test suites for executable main.java.Main.fillInitialArray
    
    ///region
    
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("fillInitialArray: arg_0 = blank string -> throw IllegalArgumentException")
    public void testFillInitialArrayThrowsIAEWithBlankString() {
        Main.fillInitialArray("   ");
    }
    ///endregion
    
    ///region EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method fillInitialArray(java.lang.String)
    
    /**
    <pre>
    Test executes conditions:
 *     {@code (fileName == null): True }
 * 
 * throws IllegalArgumentException after condition: fileName == null
 * </pre>
     */
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("fillInitialArray: fileName == null -> ThrowIllegalArgumentException")
    public void testFillInitialArray_FileNameEqualsNull() {
        Main.fillInitialArray(null);
    }
    ///endregion
    
    ///endregion
    
    ///region Test suites for executable main.java.Main.getBoundaryNumbers
    
    ///region
    
    @Test
    //@org.junit.jupiter.api.DisplayName("getBoundaryNumbers: size > 0 and others")
    public void testGetBoundaryNumbers() {
        LinkedList nodeArray = new LinkedList();
        
        ArrayList actual = ((ArrayList) Main.getBoundaryNumbers(nodeArray, 1));
        
        ArrayList expected = new ArrayList();
        assertTrue(deepEquals(expected, actual));
    }
    ///endregion
    
    ///region
    
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
    ///endregion
    
    ///region EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method getBoundaryNumbers(java.util.List, int)
    
    /**
    <pre>
    Test executes conditions:
 *     {@code (nodeArray == null): True }
 * 
 * throws IllegalArgumentException after condition: nodeArray == null
 * </pre>
     */
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("getBoundaryNumbers: nodeArray == null -> ThrowIllegalArgumentException")
    public void testGetBoundaryNumbers_NodeArrayEqualsNull() {
        Main.getBoundaryNumbers(null, -255);
    }
    
    /**
    <pre>
    Test executes conditions:
 *     {@code (nodeArray == null): False },
 *     {@code (size < 1): True }
 * 
 * throws IllegalArgumentException after condition: size < 1
 * </pre>
     */
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("getBoundaryNumbers: size < 1 -> ThrowIllegalArgumentException")
    public void testGetBoundaryNumbers_SizeLessThan1() {
        ArrayList nodeArray = new ArrayList();
        
        Main.getBoundaryNumbers(nodeArray, 0);
    }
    
    /**
    <pre>
    Test executes conditions:
 *     {@code (nodeArray == null): False },
 *     {@code (size < 1): False }
 * invokes:
 *     {@link java.util.List#contains(java.lang.Object)} once
 * executes conditions:
 *     {@code (nodeArray.contains(null)): True }
 * 
 * throws IllegalArgumentException after condition: nodeArray.contains(null)
 * </pre>
     */
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("getBoundaryNumbers: nodeArray.contains(null) -> ThrowIllegalArgumentException")
    public void testGetBoundaryNumbers_NodeArrayContains() {
        ArrayList nodeArray = new ArrayList();
        nodeArray.add(null);
        nodeArray.add(null);
        nodeArray.add(null);
        
        Main.getBoundaryNumbers(nodeArray, 1);
    }
    ///endregion
    
    ///endregion
    
    ///region Test suites for executable main.java.Main.computeLeadingNumbers
    
    ///region
    
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("computeLeadingNumbers: rank < 0, size < 1 and others -> throw IllegalArgumentException")
    public void testComputeLeadingNumbersThrowsIAEWithCornerCase() {
        List allBoundaryNumbers = emptyList();
        
        Main.computeLeadingNumbers(allBoundaryNumbers, -1, 0);
    }
    
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("computeLeadingNumbers: rank = Int.MAX_VALUE, size < 1 and others -> throw IllegalArgumentException")
    public void testComputeLeadingNumbersThrowsIAEWithCornerCases() {
        List allBoundaryNumbers = emptyList();
        
        Main.computeLeadingNumbers(allBoundaryNumbers, Integer.MAX_VALUE, 0);
    }
    ///endregion
    
    ///region
    
    @Test
    public void testComputeLeadingNumbers1() {
        ArrayList allBoundaryNumbers = new ArrayList();
        Integer integer = 0;
        allBoundaryNumbers.add(integer);
        
        ArrayList actual = ((ArrayList) Main.computeLeadingNumbers(allBoundaryNumbers, 2, 3));
        
        ArrayList expected = new ArrayList();
        assertTrue(deepEquals(expected, actual));
    }
    ///endregion
    
    ///region EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method computeLeadingNumbers(java.util.List, int, int)
    
    /**
    <pre>
    Test executes conditions:
 *     {@code (allBoundaryNumbers == null): True }
 * 
 * throws IllegalArgumentException after condition: allBoundaryNumbers == null
 * </pre>
     */
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("computeLeadingNumbers: allBoundaryNumbers == null -> ThrowIllegalArgumentException")
    public void testComputeLeadingNumbers_AllBoundaryNumbersEqualsNull() {
        Main.computeLeadingNumbers(null, -255, -255);
    }
    
    /**
    <pre>
    Test executes conditions:
 *     {@code (allBoundaryNumbers == null): False },
 *     {@code (allBoundaryNumbers.contains(null)): True }
 * 
 * throws IllegalArgumentException after condition: allBoundaryNumbers.contains(null)
 * </pre>
     */
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("computeLeadingNumbers: allBoundaryNumbers.contains(null) -> ThrowIllegalArgumentException")
    public void testComputeLeadingNumbers_AllBoundaryNumbersContains() {
        ArrayList allBoundaryNumbers = new ArrayList();
        allBoundaryNumbers.add(null);
        allBoundaryNumbers.add(null);
        allBoundaryNumbers.add(null);
        
        Main.computeLeadingNumbers(allBoundaryNumbers, -255, -255);
    }
    
    /**
    <pre>
    Test executes conditions:
 *     {@code (allBoundaryNumbers == null): False },
 *     {@code (allBoundaryNumbers.contains(null)): False },
 *     {@code (rank < 0): False },
 *     {@code (size < 1): False },
 *     {@code (rank >= size): True }
 * 
 * throws IllegalArgumentException after condition: rank >= size
 * </pre>
     */
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("computeLeadingNumbers: rank >= size -> ThrowIllegalArgumentException")
    public void testComputeLeadingNumbers_RankGreaterOrEqualSize() {
        ArrayList allBoundaryNumbers = new ArrayList();
        
        Main.computeLeadingNumbers(allBoundaryNumbers, 128, 1);
    }
    ///endregion
    
    ///endregion
    
    ///region Test suites for executable main.java.Main.getNodeBorders
    
    ///region
    
    @Test
    //@org.junit.jupiter.api.DisplayName("getNodeBorders: size > 0 and others")
    public void testGetNodeBorders() {
        LinkedList initialArray = new LinkedList();
        
        ArrayList actual = ((ArrayList) Main.getNodeBorders(initialArray, 1));
        
        ArrayList expected = new ArrayList();
        ArrayList arrayList = new ArrayList();
        Integer integer = 0;
        arrayList.add(integer);
        expected.add(arrayList);
        ArrayList arrayList1 = new ArrayList();
        Integer integer1 = -1;
        arrayList1.add(integer1);
        expected.add(arrayList1);
        assertTrue(deepEquals(expected, actual));
    }
    
    @Test(timeout = 1000L)
    //@org.junit.jupiter.api.DisplayName("getNodeBorders: size = Int.MAX_VALUE and others")
    public void testGetNodeBordersWithCornerCase() {
        ArrayList initialArray = new ArrayList();
        
        /* This execution may take longer than the 1000 ms timeout
         and therefore fail due to exceeding the timeout. */
    }
    ///endregion
    
    ///region EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method getNodeBorders(java.util.List, int)
    
    /**
    <pre>
    Test executes conditions:
 *     {@code (initialArray == null): True }
 * 
 * throws IllegalArgumentException after condition: initialArray == null
 * </pre>
     */
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("getNodeBorders: initialArray == null -> ThrowIllegalArgumentException")
    public void testGetNodeBorders_InitialArrayEqualsNull() {
        Main.getNodeBorders(null, -255);
    }
    
    /**
    <pre>
    Test executes conditions:
 *     {@code (initialArray == null): False },
 *     {@code (size < 1): True }
 * 
 * throws IllegalArgumentException after condition: size < 1
 * </pre>
     */
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("getNodeBorders: size < 1 -> ThrowIllegalArgumentException")
    public void testGetNodeBorders_SizeLessThan1() {
        ArrayList initialArray = new ArrayList();
        
        Main.getNodeBorders(initialArray, 0);
    }
    ///endregion
    
    ///endregion
    
    ///region Test suites for executable main.java.Main.getInitialArrayFromFile
    
    ///region
    
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("getInitialArrayFromFile: arg_0 = blank string -> throw IllegalArgumentException")
    public void testGetInitialArrayFromFileThrowsIAEWithBlankString() {
        Main.getInitialArrayFromFile("   ");
    }
    ///endregion
    
    ///region EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method getInitialArrayFromFile(java.lang.String)
    
    /**
    <pre>
    Test executes conditions:
 *     {@code (fileName == null): True }
 * 
 * throws IllegalArgumentException after condition: fileName == null
 * </pre>
     */
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("getInitialArrayFromFile: fileName == null -> ThrowIllegalArgumentException")
    public void testGetInitialArrayFromFile_FileNameEqualsNull() {
        Main.getInitialArrayFromFile(null);
    }
    ///endregion
    
    ///endregion
    
    ///region Test suites for executable main.java.Main.getSentSequencesNumbers
    
    ///region
    
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("getSentSequencesNumbers: size = 0 and others -> throw IllegalArgumentException")
    public void testGetSentSequencesNumbersThrowsIAEWithCornerCase() {
        ArrayList list = new ArrayList();
        List nodeArray = synchronizedList(list);
        ArrayList leadingNumbers = new ArrayList();
        
        Main.getSentSequencesNumbers(nodeArray, leadingNumbers, 0);
    }
    ///endregion
    
    ///region
    
    @Test
    public void testGetSentSequencesNumbers1() {
        ArrayList nodeArray = new ArrayList();
        ArrayList leadingNumbers = new ArrayList();
        
        ArrayList actual = ((ArrayList) Main.getSentSequencesNumbers(nodeArray, leadingNumbers, 128));
        
        ArrayList expected = new ArrayList();
        ArrayList arrayList = new ArrayList();
        expected.add(arrayList);
        ArrayList arrayList1 = new ArrayList();
        expected.add(arrayList1);
        ArrayList arrayList2 = new ArrayList();
        expected.add(arrayList2);
        ArrayList arrayList3 = new ArrayList();
        expected.add(arrayList3);
        ArrayList arrayList4 = new ArrayList();
        expected.add(arrayList4);
        ArrayList arrayList5 = new ArrayList();
        expected.add(arrayList5);
        ArrayList arrayList6 = new ArrayList();
        expected.add(arrayList6);
        ArrayList arrayList7 = new ArrayList();
        expected.add(arrayList7);
        ArrayList arrayList8 = new ArrayList();
        expected.add(arrayList8);
        ArrayList arrayList9 = new ArrayList();
        expected.add(arrayList9);
        ArrayList arrayList10 = new ArrayList();
        expected.add(arrayList10);
        ArrayList arrayList11 = new ArrayList();
        expected.add(arrayList11);
        ArrayList arrayList12 = new ArrayList();
        expected.add(arrayList12);
        ArrayList arrayList13 = new ArrayList();
        expected.add(arrayList13);
        ArrayList arrayList14 = new ArrayList();
        expected.add(arrayList14);
        ArrayList arrayList15 = new ArrayList();
        expected.add(arrayList15);
        ArrayList arrayList16 = new ArrayList();
        expected.add(arrayList16);
        ArrayList arrayList17 = new ArrayList();
        expected.add(arrayList17);
        ArrayList arrayList18 = new ArrayList();
        expected.add(arrayList18);
        ArrayList arrayList19 = new ArrayList();
        expected.add(arrayList19);
        ArrayList arrayList20 = new ArrayList();
        expected.add(arrayList20);
        ArrayList arrayList21 = new ArrayList();
        expected.add(arrayList21);
        ArrayList arrayList22 = new ArrayList();
        expected.add(arrayList22);
        ArrayList arrayList23 = new ArrayList();
        expected.add(arrayList23);
        ArrayList arrayList24 = new ArrayList();
        expected.add(arrayList24);
        ArrayList arrayList25 = new ArrayList();
        expected.add(arrayList25);
        ArrayList arrayList26 = new ArrayList();
        expected.add(arrayList26);
        ArrayList arrayList27 = new ArrayList();
        expected.add(arrayList27);
        ArrayList arrayList28 = new ArrayList();
        expected.add(arrayList28);
        ArrayList arrayList29 = new ArrayList();
        expected.add(arrayList29);
        ArrayList arrayList30 = new ArrayList();
        expected.add(arrayList30);
        ArrayList arrayList31 = new ArrayList();
        expected.add(arrayList31);
        ArrayList arrayList32 = new ArrayList();
        expected.add(arrayList32);
        ArrayList arrayList33 = new ArrayList();
        expected.add(arrayList33);
        ArrayList arrayList34 = new ArrayList();
        expected.add(arrayList34);
        ArrayList arrayList35 = new ArrayList();
        expected.add(arrayList35);
        ArrayList arrayList36 = new ArrayList();
        expected.add(arrayList36);
        ArrayList arrayList37 = new ArrayList();
        expected.add(arrayList37);
        ArrayList arrayList38 = new ArrayList();
        expected.add(arrayList38);
        ArrayList arrayList39 = new ArrayList();
        expected.add(arrayList39);
        ArrayList arrayList40 = new ArrayList();
        expected.add(arrayList40);
        ArrayList arrayList41 = new ArrayList();
        expected.add(arrayList41);
        ArrayList arrayList42 = new ArrayList();
        expected.add(arrayList42);
        ArrayList arrayList43 = new ArrayList();
        expected.add(arrayList43);
        ArrayList arrayList44 = new ArrayList();
        expected.add(arrayList44);
        ArrayList arrayList45 = new ArrayList();
        expected.add(arrayList45);
        ArrayList arrayList46 = new ArrayList();
        expected.add(arrayList46);
        ArrayList arrayList47 = new ArrayList();
        expected.add(arrayList47);
        ArrayList arrayList48 = new ArrayList();
        expected.add(arrayList48);
        ArrayList arrayList49 = new ArrayList();
        expected.add(arrayList49);
        ArrayList arrayList50 = new ArrayList();
        expected.add(arrayList50);
        ArrayList arrayList51 = new ArrayList();
        expected.add(arrayList51);
        ArrayList arrayList52 = new ArrayList();
        expected.add(arrayList52);
        ArrayList arrayList53 = new ArrayList();
        expected.add(arrayList53);
        ArrayList arrayList54 = new ArrayList();
        expected.add(arrayList54);
        ArrayList arrayList55 = new ArrayList();
        expected.add(arrayList55);
        ArrayList arrayList56 = new ArrayList();
        expected.add(arrayList56);
        ArrayList arrayList57 = new ArrayList();
        expected.add(arrayList57);
        ArrayList arrayList58 = new ArrayList();
        expected.add(arrayList58);
        ArrayList arrayList59 = new ArrayList();
        expected.add(arrayList59);
        ArrayList arrayList60 = new ArrayList();
        expected.add(arrayList60);
        ArrayList arrayList61 = new ArrayList();
        expected.add(arrayList61);
        ArrayList arrayList62 = new ArrayList();
        expected.add(arrayList62);
        ArrayList arrayList63 = new ArrayList();
        expected.add(arrayList63);
        ArrayList arrayList64 = new ArrayList();
        expected.add(arrayList64);
        ArrayList arrayList65 = new ArrayList();
        expected.add(arrayList65);
        ArrayList arrayList66 = new ArrayList();
        expected.add(arrayList66);
        ArrayList arrayList67 = new ArrayList();
        expected.add(arrayList67);
        ArrayList arrayList68 = new ArrayList();
        expected.add(arrayList68);
        ArrayList arrayList69 = new ArrayList();
        expected.add(arrayList69);
        ArrayList arrayList70 = new ArrayList();
        expected.add(arrayList70);
        ArrayList arrayList71 = new ArrayList();
        expected.add(arrayList71);
        ArrayList arrayList72 = new ArrayList();
        expected.add(arrayList72);
        ArrayList arrayList73 = new ArrayList();
        expected.add(arrayList73);
        ArrayList arrayList74 = new ArrayList();
        expected.add(arrayList74);
        ArrayList arrayList75 = new ArrayList();
        expected.add(arrayList75);
        ArrayList arrayList76 = new ArrayList();
        expected.add(arrayList76);
        ArrayList arrayList77 = new ArrayList();
        expected.add(arrayList77);
        ArrayList arrayList78 = new ArrayList();
        expected.add(arrayList78);
        ArrayList arrayList79 = new ArrayList();
        expected.add(arrayList79);
        ArrayList arrayList80 = new ArrayList();
        expected.add(arrayList80);
        ArrayList arrayList81 = new ArrayList();
        expected.add(arrayList81);
        ArrayList arrayList82 = new ArrayList();
        expected.add(arrayList82);
        ArrayList arrayList83 = new ArrayList();
        expected.add(arrayList83);
        ArrayList arrayList84 = new ArrayList();
        expected.add(arrayList84);
        ArrayList arrayList85 = new ArrayList();
        expected.add(arrayList85);
        ArrayList arrayList86 = new ArrayList();
        expected.add(arrayList86);
        ArrayList arrayList87 = new ArrayList();
        expected.add(arrayList87);
        ArrayList arrayList88 = new ArrayList();
        expected.add(arrayList88);
        ArrayList arrayList89 = new ArrayList();
        expected.add(arrayList89);
        ArrayList arrayList90 = new ArrayList();
        expected.add(arrayList90);
        ArrayList arrayList91 = new ArrayList();
        expected.add(arrayList91);
        ArrayList arrayList92 = new ArrayList();
        expected.add(arrayList92);
        ArrayList arrayList93 = new ArrayList();
        expected.add(arrayList93);
        ArrayList arrayList94 = new ArrayList();
        expected.add(arrayList94);
        ArrayList arrayList95 = new ArrayList();
        expected.add(arrayList95);
        ArrayList arrayList96 = new ArrayList();
        expected.add(arrayList96);
        ArrayList arrayList97 = new ArrayList();
        expected.add(arrayList97);
        ArrayList arrayList98 = new ArrayList();
        expected.add(arrayList98);
        ArrayList arrayList99 = new ArrayList();
        expected.add(arrayList99);
        ArrayList arrayList100 = new ArrayList();
        expected.add(arrayList100);
        ArrayList arrayList101 = new ArrayList();
        expected.add(arrayList101);
        ArrayList arrayList102 = new ArrayList();
        expected.add(arrayList102);
        ArrayList arrayList103 = new ArrayList();
        expected.add(arrayList103);
        ArrayList arrayList104 = new ArrayList();
        expected.add(arrayList104);
        ArrayList arrayList105 = new ArrayList();
        expected.add(arrayList105);
        ArrayList arrayList106 = new ArrayList();
        expected.add(arrayList106);
        ArrayList arrayList107 = new ArrayList();
        expected.add(arrayList107);
        ArrayList arrayList108 = new ArrayList();
        expected.add(arrayList108);
        ArrayList arrayList109 = new ArrayList();
        expected.add(arrayList109);
        ArrayList arrayList110 = new ArrayList();
        expected.add(arrayList110);
        ArrayList arrayList111 = new ArrayList();
        expected.add(arrayList111);
        ArrayList arrayList112 = new ArrayList();
        expected.add(arrayList112);
        ArrayList arrayList113 = new ArrayList();
        expected.add(arrayList113);
        ArrayList arrayList114 = new ArrayList();
        expected.add(arrayList114);
        ArrayList arrayList115 = new ArrayList();
        expected.add(arrayList115);
        ArrayList arrayList116 = new ArrayList();
        expected.add(arrayList116);
        ArrayList arrayList117 = new ArrayList();
        expected.add(arrayList117);
        ArrayList arrayList118 = new ArrayList();
        expected.add(arrayList118);
        ArrayList arrayList119 = new ArrayList();
        expected.add(arrayList119);
        ArrayList arrayList120 = new ArrayList();
        expected.add(arrayList120);
        ArrayList arrayList121 = new ArrayList();
        expected.add(arrayList121);
        ArrayList arrayList122 = new ArrayList();
        expected.add(arrayList122);
        ArrayList arrayList123 = new ArrayList();
        expected.add(arrayList123);
        ArrayList arrayList124 = new ArrayList();
        expected.add(arrayList124);
        ArrayList arrayList125 = new ArrayList();
        expected.add(arrayList125);
        ArrayList arrayList126 = new ArrayList();
        expected.add(arrayList126);
        ArrayList arrayList127 = new ArrayList();
        expected.add(arrayList127);
        assertTrue(deepEquals(expected, actual));
    }
    ///endregion
    
    ///region EXPLICITLY THROWN UNCHECKED EXCEPTIONS for method getSentSequencesNumbers(java.util.List, java.util.List, int)
    
    /**
    <pre>
    Test executes conditions:
 *     {@code (nodeArray == null): True }
 * 
 * throws IllegalArgumentException after condition: nodeArray == null
 * </pre>
     */
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("getSentSequencesNumbers: nodeArray == null -> ThrowIllegalArgumentException")
    public void testGetSentSequencesNumbers_NodeArrayEqualsNull() {
        Main.getSentSequencesNumbers(null, null, -255);
    }
    
    /**
    <pre>
    Test executes conditions:
 *     {@code (nodeArray == null): False },
 *     {@code (leadingNumbers == null): True }
 * 
 * throws IllegalArgumentException after condition: leadingNumbers == null
 * </pre>
     */
    @Test(expected = IllegalArgumentException.class)
    //@org.junit.jupiter.api.DisplayName("getSentSequencesNumbers: leadingNumbers == null -> ThrowIllegalArgumentException")
    public void testGetSentSequencesNumbers_LeadingNumbersEqualsNull() {
        ArrayList nodeArray = new ArrayList();
        
        Main.getSentSequencesNumbers(nodeArray, null, -255);
    }
    ///endregion
    
    ///endregion
    
    ///region Data providers and utils methods
    
    static class FieldsPair {
        final Object o1;
        final Object o2;
    
        public FieldsPair(Object o1, Object o2) {
            this.o1 = o1;
            this.o2 = o2;
        }
    
        @Override
        public boolean equals(Object o) {
            if (this == o) return true;
            if (o == null || getClass() != o.getClass()) return false;
            FieldsPair that = (FieldsPair) o;
            return Objects.equals(o1, that.o1) && Objects.equals(o2, that.o2);
        }
    
        @Override
        public int hashCode() {
            return Objects.hash(o1, o2);
        }
    }
    
    private boolean deepEquals(Object o1, Object o2) {
        return deepEquals(o1, o2, new java.util.HashSet<>());
    }
    
    private boolean deepEquals(Object o1, Object o2, java.util.Set<FieldsPair> visited) {
        visited.add(new FieldsPair(o1, o2));
    
        if (o1 == o2) {
            return true;
        }
    
        if (o1 == null || o2 == null) {
            return false;
        }
    
        if (o1 instanceof Iterable) {
            if (!(o2 instanceof Iterable)) {
                return false;
            }
    
            return iterablesDeepEquals((Iterable<?>) o1, (Iterable<?>) o2, visited);
        }
        
        if (o2 instanceof Iterable) {
            return false;
        }
        
        if (o1 instanceof java.util.stream.Stream) {
            if (!(o2 instanceof java.util.stream.Stream)) {
                return false;
            }
    
            return streamsDeepEquals((java.util.stream.Stream<?>) o1, (java.util.stream.Stream<?>) o2, visited);
        }
    
        if (o2 instanceof java.util.stream.Stream) {
            return false;
        }
    
        if (o1 instanceof java.util.Map) {
            if (!(o2 instanceof java.util.Map)) {
                return false;
            }
    
            return mapsDeepEquals((java.util.Map<?, ?>) o1, (java.util.Map<?, ?>) o2, visited);
        }
        
        if (o2 instanceof java.util.Map) {
            return false;
        }
    
        Class<?> firstClass = o1.getClass();
        if (firstClass.isArray()) {
            if (!o2.getClass().isArray()) {
                return false;
            }
    
            // Primitive arrays should not appear here
            return arraysDeepEquals(o1, o2, visited);
        }
    
        // common classes
    
        // check if class has custom equals method (including wrappers and strings)
        // It is very important to check it here but not earlier because iterables and maps also have custom equals 
        // based on elements equals 
        if (hasCustomEquals(firstClass)) {
            return o1.equals(o2);
        }
    
        // common classes without custom equals, use comparison by fields
        final java.util.List<java.lang.reflect.Field> fields = new java.util.ArrayList<>();
        while (firstClass != Object.class) {
            fields.addAll(java.util.Arrays.asList(firstClass.getDeclaredFields()));
            // Interface should not appear here
            firstClass = firstClass.getSuperclass();
        }
    
        for (java.lang.reflect.Field field : fields) {
            field.setAccessible(true);
            try {
                final Object field1 = field.get(o1);
                final Object field2 = field.get(o2);
                if (!visited.contains(new FieldsPair(field1, field2)) && !deepEquals(field1, field2, visited)) {
                    return false;
                }
            } catch (IllegalArgumentException e) {
                return false;
            } catch (IllegalAccessException e) {
                // should never occur because field was set accessible
                return false;
            }
        }
    
        return true;
    }
    
    private boolean arraysDeepEquals(Object arr1, Object arr2, java.util.Set<FieldsPair> visited) {
        final int length = java.lang.reflect.Array.getLength(arr1);
        if (length != java.lang.reflect.Array.getLength(arr2)) {
            return false;
        }
    
        for (int i = 0; i < length; i++) {
            if (!deepEquals(java.lang.reflect.Array.get(arr1, i), java.lang.reflect.Array.get(arr2, i), visited)) {
                return false;
            }
        }
    
        return true;
    }
    
    private boolean iterablesDeepEquals(Iterable<?> i1, Iterable<?> i2, java.util.Set<FieldsPair> visited) {
        final java.util.Iterator<?> firstIterator = i1.iterator();
        final java.util.Iterator<?> secondIterator = i2.iterator();
        while (firstIterator.hasNext() && secondIterator.hasNext()) {
            if (!deepEquals(firstIterator.next(), secondIterator.next(), visited)) {
                return false;
            }
        }
    
        if (firstIterator.hasNext()) {
            return false;
        }
    
        return !secondIterator.hasNext();
    }
    
    private boolean streamsDeepEquals(
        java.util.stream.Stream<?> s1, 
        java.util.stream.Stream<?> s2, 
        java.util.Set<FieldsPair> visited
    ) {
        final java.util.Iterator<?> firstIterator = s1.iterator();
        final java.util.Iterator<?> secondIterator = s2.iterator();
        while (firstIterator.hasNext() && secondIterator.hasNext()) {
            if (!deepEquals(firstIterator.next(), secondIterator.next(), visited)) {
                return false;
            }
        }
    
        if (firstIterator.hasNext()) {
            return false;
        }
    
        return !secondIterator.hasNext();
    }
    
    private boolean mapsDeepEquals(
        java.util.Map<?, ?> m1, 
        java.util.Map<?, ?> m2, 
        java.util.Set<FieldsPair> visited
    ) {
        final java.util.Iterator<? extends java.util.Map.Entry<?, ?>> firstIterator = m1.entrySet().iterator();
        final java.util.Iterator<? extends java.util.Map.Entry<?, ?>> secondIterator = m2.entrySet().iterator();
        while (firstIterator.hasNext() && secondIterator.hasNext()) {
            final java.util.Map.Entry<?, ?> firstEntry = firstIterator.next();
            final java.util.Map.Entry<?, ?> secondEntry = secondIterator.next();
    
            if (!deepEquals(firstEntry.getKey(), secondEntry.getKey(), visited)) {
                return false;
            }
    
            if (!deepEquals(firstEntry.getValue(), secondEntry.getValue(), visited)) {
                return false;
            }
        }
    
        if (firstIterator.hasNext()) {
            return false;
        }
    
        return !secondIterator.hasNext();
    }
    
    private boolean hasCustomEquals(Class<?> clazz) {
        while (!Object.class.equals(clazz)) {
            try {
                clazz.getDeclaredMethod("equals", Object.class);
                return true;
            } catch (Exception e) { 
                // Interface should not appear here
                clazz = clazz.getSuperclass();
            }
        }
    
        return false;
    }
    ///endregion
}
