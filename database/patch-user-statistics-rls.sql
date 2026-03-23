-- ============================================================================
-- PATCH: Fix user_statistics RLS policies
-- ============================================================================
-- This patch fixes the "new row violates row-level security policy" error
-- that occurs when the trigger tries to update user_statistics.
--
-- ISSUE: The trigger update_user_statistics() performs INSERT and UPDATE 
-- operations, but only SELECT policy existed.
--
-- SOLUTION: Add INSERT and UPDATE policies to allow users to manage their 
-- own statistics row.
--
-- WHEN TO USE: If you already applied improvements.sql and are getting
-- the RLS error when saving catches.
-- ============================================================================

-- Add missing INSERT policy (for new users)
DROP POLICY IF EXISTS "Users insert own statistics" ON user_statistics;
CREATE POLICY "Users insert own statistics" ON user_statistics 
    FOR INSERT WITH CHECK (auth.uid() = user_id);

-- Add missing UPDATE policy (for existing users)
DROP POLICY IF EXISTS "Users update own statistics" ON user_statistics;
CREATE POLICY "Users update own statistics" ON user_statistics 
    FOR UPDATE USING (auth.uid() = user_id);

-- ============================================================================
-- VERIFICATION
-- ============================================================================
-- After applying this patch, verify all policies exist:
-- 
-- SELECT schemaname, tablename, policyname, cmd 
-- FROM pg_policies 
-- WHERE tablename = 'user_statistics';
--
-- Expected result:
-- | policyname                      | cmd    |
-- |---------------------------------|--------|
-- | Users see own statistics        | SELECT |
-- | Users insert own statistics     | INSERT |
-- | Users update own statistics     | UPDATE |
-- ============================================================================
